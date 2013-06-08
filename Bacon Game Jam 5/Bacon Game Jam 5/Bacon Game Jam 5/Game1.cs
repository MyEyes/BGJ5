using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Bacon_Game_Jam_5
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera cam;
        Map map;
        Player player;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            cam = new Camera(new Vector2(100, 100), GraphicsDevice.Viewport.Bounds);
            map = new Map(Content);
            map.lightMap = new Lightmap(GraphicsDevice, Content);
            map.lightMap.AmbientColor = new Color(50,50,50);
            player = new Player(new Vector2(5,5), map, Content);
            map.Objects.Add(player);
            Random rand = new Random();
            for (int x = 0; x < 30; x++)
            {
                Enemy enemy = new Enemy(new Vector2((float)rand.NextDouble() * Map.SizeX * Map.TileSize, (float)rand.NextDouble() * Map.SizeY * Map.TileSize), map, Content);
                map.Objects.Add(enemy);
            }
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            KeyboardState keyboard = Keyboard.GetState();
            map.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (keyboard.IsKeyDown(Keys.A))
                player.Move(new Vector2(-1, 0));
            if (keyboard.IsKeyDown(Keys.D))
                player.Move(new Vector2(1, 0));
            if (keyboard.IsKeyDown(Keys.W))
                player.Move(new Vector2(0,-1));
            if (keyboard.IsKeyDown(Keys.S))
                player.Move(new Vector2(0, 1));

            cam.SetPosition(player.Position);
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            map.lightMap.DrawLights(cam);

            GraphicsDevice.Clear(Color.Black);

            map.Draw(cam, spriteBatch);
            map.lightMap.DrawLightmap(spriteBatch);
            
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
