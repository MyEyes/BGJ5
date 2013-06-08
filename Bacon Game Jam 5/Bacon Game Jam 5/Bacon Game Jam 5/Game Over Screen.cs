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
    class Game_Over_Screen:IScreen
    {
        SpriteFont _font;
        GraphicsDevice _device;

        Texture2D oldScreen;
        float fadeOut;

        public ScreenManager Manager { get; set; }

        public Game_Over_Screen(Texture2D oldScreen)
        {
            fadeOut = 5;
            this.oldScreen = oldScreen;
        }

        public void Initialize(GraphicsDevice device, ContentManager Content)
        {
            _font = Content.Load<SpriteFont>("font");
            _device = device;
        }

        public void Draw(SpriteBatch batch)
        {
            _device.Clear(Color.Black);
            batch.Begin();

            batch.Draw(oldScreen, Vector2.Zero, Color.Lerp(Color.Black, Color.White, fadeOut / 5.0f));
            batch.DrawString(_font, "The light is out", new Vector2(100, 200), Color.Lerp(Color.Black, Color.White, 1-fadeOut / 5.0f));

            batch.DrawString(_font, "Press SPACE to retry!", new Vector2(100, 280), Color.Lerp(Color.Black, Color.White, 1-fadeOut / 5.0f));
            batch.End();
        }

        public void Update(float seconds)
        {
            if (fadeOut > 0)
                fadeOut -= seconds;
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Space))
            {
                Manager.Remove(this);
                Manager.Add(new GameScreen());
            }
        }
    }
}
