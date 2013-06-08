using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bacon_Game_Jam_5
{
    class GameScreen:IScreen
    {
        Camera cam;
        Map map;
        Player player;
        Enemy lastEnemy;
        GraphicsDevice GraphicsDevice;

        float winCountdown = 1.5f;

        RenderTarget2D sceneBuffer;

        public ScreenManager Manager { get; set; }

        public void Initialize(GraphicsDevice GraphicsDevice, ContentManager Content)
        {
            this.GraphicsDevice = GraphicsDevice;

            sceneBuffer = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            cam = new Camera(new Vector2(100, 100), GraphicsDevice.Viewport.Bounds);
            map = new Map(Content);
            map.lightMap = new Lightmap(GraphicsDevice, Content);
            map.lightMap.AmbientColor = new Color(30, 30, 30);
            
            player = new Player(new Vector2(64, 64), map, Content);
            map.Objects.Add(player);

            Random rand = new Random();
            
            for (int x = 0; x < 15; x++)
            {
                int offset = 30*Map.TileSize;
                Enemy enemy = new Enemy(new Vector2(offset + (float)rand.NextDouble() * (Map.SizeX * Map.TileSize - offset), offset + (float)rand.NextDouble() * (Map.SizeY * Map.TileSize - offset)), map, Content);
                map.Objects.Add(enemy);
            }
             /*
            Enemy enemy = new Enemy(new Vector2(160, 160), map, Content);
            enemy.Health = 200;
            map.Objects.Add(enemy);
            lastEnemy = enemy;
              */

            for (int x = 0; x < 20; x++)
            {
                LightRefill refill = new LightRefill(new Vector2((float)rand.NextDouble() * Map.SizeX * Map.TileSize, (float)rand.NextDouble() * Map.SizeY * Map.TileSize), map, Content);
                map.Objects.Add(refill);
            }

            for (int x = 0; x < 4; x++)
            {
                RageTrigger rt = new RageTrigger(new Vector2((float)rand.NextDouble() * Map.SizeX * Map.TileSize, (float)rand.NextDouble() * Map.SizeY * Map.TileSize), map, Content);
                map.Objects.Add(rt);
            }
        }

        public void Update(float seconds)
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.A))
                player.Move(new Vector2(-2.5f, 0));
            if (keyboard.IsKeyDown(Keys.D))
                player.Move(new Vector2(2.5f, 0));
            if (keyboard.IsKeyDown(Keys.W))
                player.Move(new Vector2(0, -2.5f));
            if (keyboard.IsKeyDown(Keys.S))
                player.Move(new Vector2(0, 2.5f));

            map.Update(seconds);

            if (player.Health < 0)
            {
                Manager.Remove(this);
                Manager.Add(new Game_Over_Screen(sceneBuffer));
            }

            if(lastEnemy==null)
            {
                int EnemyCount = 0;
                Enemy llEnemy=null;
                for (int x = 0; x < map.Objects.Count; x++)
                {
                    if (map.Objects[x] is Enemy)
                    {
                        llEnemy = map.Objects[x] as Enemy;
                        EnemyCount++;
                    }
                }
                if (EnemyCount == 1)
                {
                    lastEnemy = llEnemy;
                    llEnemy.Health = 20000;
                }
                else if (EnemyCount == 0)
                {
                    Manager.Remove(this);
                    Manager.Add(new YouWinScreen(sceneBuffer, player.Position));
                }
            }

            if (lastEnemy != null && lastEnemy.Health <= 0)
            {
                winCountdown -= seconds;
                if (winCountdown <= 0)
                {
                    Manager.Remove(this);
                    Manager.Add(new YouWinScreen(sceneBuffer, Vector2.Transform(lastEnemy.Position, cam.ViewMatrix)));
                }
            }


            cam.SetPosition(player.Position);

        }

        public void Draw(SpriteBatch batch)
        {
            map.lightMap.DrawLights(cam, batch, map);

            GraphicsDevice.SetRenderTarget(sceneBuffer);
            GraphicsDevice.Clear(Color.Black);

            map.Draw(cam, batch, BlendState.Opaque);
            map.lightMap.DrawLightmap(batch);
            GraphicsDevice.SetRenderTarget(null);

            batch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            batch.Draw(sceneBuffer, Vector2.Zero, Color.White);
            batch.End();
        }
    }
}
