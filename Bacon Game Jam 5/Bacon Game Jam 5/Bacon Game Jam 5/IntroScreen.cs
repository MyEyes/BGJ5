using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bacon_Game_Jam_5
{
    class IntroScreen:IScreen
    {
        string Text = "For thousands of years,\nthe lights of the sky lived in paradise                \nand life was perfect.";
        SpriteFont _font;
        
        GraphicsDevice _device;
        ContentManager _content;

        Map _dummyMap;
        Camera _dummyCam;

        public ScreenManager Manager { get; set; }

        Player player1;
        Player player2;
        Player player3;
        Player player4;
        Enemy enemy;

        int phase = 0;
        float phaseCountdown = 10;
        float PhaseTime = 10;
        Vector2 offset = new Vector2(2000, 2000);

        public void Initialize(GraphicsDevice device, ContentManager Content)
        {
            _content = Content;
            _font = Content.Load<SpriteFont>("font");
            _device = device;
            _dummyMap = new Map(Content);
            _dummyCam = new Camera(new Vector2(400, 240)+offset, device.Viewport.Bounds);
            _dummyMap.lightMap = new Lightmap(device, Content);
            _dummyMap.lightMap.AmbientColor = new Color(200, 200, 200);
            player1 = new Player(new Vector2(120, 200)+offset, _dummyMap, Content);
            player2 = new Player(new Vector2(160, 170)+offset, _dummyMap, Content);
            player3 = new Player(new Vector2(180, 230)+offset, _dummyMap, Content);
            player4 = new Player(new Vector2(64, 64), _dummyMap, Content);
            _dummyMap.Objects.Add(player1);
            _dummyMap.Objects.Add(player2);
            _dummyMap.Objects.Add(player3);
            _dummyMap.Objects.Add(player4);
        }

        public void Update(float seconds)
        {

            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Space))
            {
                if (phaseCountdown <= 0)
                    NextPhase();
                else
                    seconds *= 3;

            }
            if (state.IsKeyDown(Keys.Escape))
            {
                Manager.Remove(this);
                Manager.Add(new GameScreen());
            }
            
            if (phaseCountdown > 0)
                phaseCountdown -= seconds;

            if (phase == 0)
            {
                player1.Health = 250;
                player2.Health = 250;
                player3.Health = 250;
                player4.Health = 250;
            }
            else if (phase == 1)
            {
                enemy.Health = 10000;
                enemy.targetPos = new Vector2(500, 500)+offset;
                enemy.countDown = 1000;
                if(phaseCountdown<5)
                    _dummyMap.lightMap.AmbientColor=Color.Lerp(new Color(30, 30, 30),new Color(200, 200, 200), phaseCountdown/5.0f);
            }
            else if (phase == 2)
            {
                _dummyCam.Move((player4.Position - _dummyCam.Position)*0.01f);
                player4.Health = 250;
            }

            _dummyMap.Update(seconds);

        }

        public void NextPhase()
        {
            switch (phase)
            {
                case 0:
                    _font = _content.Load<SpriteFont>("DarkFont");
                    phase = 1;
                    phaseCountdown = 15;
                    PhaseTime = 15;
                    Text = "But then...                                                                                    \nDarkness came\n                                                     \nAnd it swallowed up all that was good!";
                    enemy = new Enemy(new Vector2(2000, 2000)+offset, _dummyMap, _content);
                    enemy.targetPos = new Vector2(500, 500)+offset;
                    enemy.speed = 2;
                    enemy.countDown = 1000;
                    enemy.collides = false;
                    enemy.follow = false;
                    _dummyMap.Objects.Add(enemy);
                    break;
                case 1:
                    phase = 2;
                    phaseCountdown = 20;
                    PhaseTime = 20;
                    Text = "You are the last hope...                  \nYou are the last light\nthat has not been found!                         \nHelp your brothers and sisters!                         \nLET THE LIGHTS OUT!";
                    enemy = new Enemy(player4.Position+new Vector2(1000,0), _dummyMap, _content);
                    enemy.targetPos = player4.Position+new Vector2(500,0);
                    enemy.speed = 2;
                    enemy.countDown = 1000;
                    enemy.Health = 3000;
                    enemy.collides = false;
                    enemy.follow = false;
                    _dummyMap.Objects.Add(enemy);
                    enemy = new Enemy(player4.Position+new Vector2(0,1000), _dummyMap, _content);
                    enemy.targetPos = player4.Position+new Vector2(0,500);
                    enemy.speed = 2;
                    enemy.Health = 3000;
                    enemy.countDown = 1000;
                    enemy.collides = false;
                    enemy.follow = false;
                    _dummyMap.Objects.Add(enemy);
                    break;
                case 2:
                    Manager.Remove(this);
                    Manager.Add(new GameScreen());
                    break;

            }
        }

        public void Draw(SpriteBatch batch)
        {
            _dummyMap.lightMap.DrawLights(_dummyCam, batch, _dummyMap);
            _device.Clear(Color.Black);
            _dummyMap.Draw(_dummyCam, batch, BlendState.AlphaBlend);
            _dummyMap.lightMap.DrawLightmap(batch);

            batch.Begin();
            int length = (int)(Text.Length * (PhaseTime - phaseCountdown) / PhaseTime);
            length = length >= 0 ? length < Text.Length ? length : Text.Length : 0;
            Color color;
            if (phase == 0 || (phase==1&&phaseCountdown > 10))
                color=Color.Black;
            else if(phase==1 && phaseCountdown>2)
                color=Color.Lerp(Color.White,Color.Black,(phaseCountdown-2)/8.0f);
            else
                color=Color.White;

            batch.DrawString(_font, Text.Substring(0, length), new Vector2(10, 10), color);
            if (phaseCountdown <= 0 && phase==2)
                batch.DrawString(_font, "Press Space to START", new Vector2(20, 430), color);
            else if(phaseCountdown<=0)
                batch.DrawString(_font, "Press Space to continue", new Vector2(20, 430), color);
            batch.DrawString(_font, "Esc ->", new Vector2(660, 430), color);

            batch.End();
        }


    }
}
