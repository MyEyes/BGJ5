using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Bacon_Game_Jam_5
{
    class YouWinScreen : IScreen
    {
        SpriteFont _font;
        GraphicsDevice _device;

        Texture2D oldScreen;
        Vector2 origin;

        VertexPositionTexture[] _vertices;
        float[] fallSpeed;

        Effect _effect;

        Random rand = new Random();

        public ScreenManager Manager { get; set; }

        RasterizerState _wireFrame;

        SoundEffectInstance _clap;
        SoundEffectInstance _fanfare;

        Texture2D BACON;

        public YouWinScreen(Texture2D oldScreen, Vector2 LastEnemyPosition)
        {
            origin = LastEnemyPosition;
            this.oldScreen = oldScreen;
        }

        public void Subdivide(List<VertexPositionTexture> vertices, Rectangle rect, int times, Vector2 concentrationPoint)
        {
            if (times == 0)
            {
                bool orientation = rand.NextDouble() > 0.5f;
                int vertexCount = vertices.Count;

                vertices.Add(new VertexPositionTexture(new Vector3(rect.Left, rect.Bottom, 0.1f), new Vector2((float)rect.Left / _device.Viewport.Width, (float)rect.Bottom / _device.Viewport.Height)));
                vertices.Add(new VertexPositionTexture(new Vector3(rect.Right, rect.Bottom, 0.1f), new Vector2((float)rect.Right / _device.Viewport.Width, (float)rect.Bottom / _device.Viewport.Height)));
                vertices.Add(new VertexPositionTexture(new Vector3(rect.Right, rect.Y, 0.1f), new Vector2((float)rect.Right / _device.Viewport.Width, (float)rect.Y / _device.Viewport.Height)));
                vertices.Add(new VertexPositionTexture(new Vector3(rect.Left, rect.Bottom, 0.1f), new Vector2((float)rect.Left / _device.Viewport.Width, (float)rect.Bottom / _device.Viewport.Height)));
                vertices.Add(new VertexPositionTexture(new Vector3(rect.Right, rect.Y, 0.1f), new Vector2((float)rect.Right / _device.Viewport.Width, (float)rect.Y / _device.Viewport.Height)));
                vertices.Add(new VertexPositionTexture(new Vector3(rect.X, rect.Y, 0.1f), new Vector2((float)rect.X / _device.Viewport.Width, (float)rect.Y / _device.Viewport.Height)));

                if (orientation)
                {
                    vertices[vertexCount + 2] = vertices[vertexCount + 5];
                    vertices[vertexCount + 3] = vertices[vertexCount + 1];
                }
                return;
            }


            Vector2 center = new Vector2(rect.Center.X, rect.Center.Y);
            Vector2 dir = concentrationPoint - center;
            dir.Normalize();
            dir.X *= rect.Width / 3 * (0.5f + (float)rand.NextDouble() * 0.5f);
            dir.Y *= rect.Height / 3 * (0.5f + (float)rand.NextDouble() * 0.5f);
            center += dir;
            Subdivide(vertices, new Rectangle(rect.X, rect.Y, (int)center.X - rect.X, (int)center.Y - rect.Y), times - 1, concentrationPoint);
            Subdivide(vertices, new Rectangle((int)center.X, rect.Y, rect.Right - (int)center.X, (int)center.Y - rect.Y), times - 1, concentrationPoint);
            Subdivide(vertices, new Rectangle(rect.X, (int)center.Y, (int)center.X - rect.X, rect.Bottom - (int)center.Y), times - 1, concentrationPoint);
            Subdivide(vertices, new Rectangle((int)center.X, (int)center.Y, rect.Right - (int)center.X, rect.Bottom - (int)center.Y), times - 1, concentrationPoint);


            _wireFrame = new RasterizerState();
            _wireFrame.CullMode = CullMode.None;
            _wireFrame.FillMode = FillMode.WireFrame;
        }

        public void Initialize(GraphicsDevice device, ContentManager Content)
        {
            _font = Content.Load<SpriteFont>("font");
            _device = device;

            _effect = Content.Load<Effect>("ShatterEffect");
            Camera cam = new Camera(new Vector2(device.Viewport.Width / 2, device.Viewport.Height / 2), device.Viewport.Bounds);
            _effect.Parameters["World"].SetValue(Matrix.Identity);
            _effect.Parameters["View"].SetValue(cam.ViewMatrix);
            _effect.Parameters["Projection"].SetValue(cam.ProjectionMatrix);
            _effect.Parameters["Texture"].SetValue(oldScreen);

            List<VertexPositionTexture> vertices = new List<VertexPositionTexture>();
            Subdivide(vertices, _device.Viewport.Bounds, 5, origin);
            _vertices = vertices.ToArray();
            fallSpeed = new float[_vertices.Length];
            float diag = new Vector2(_device.Viewport.Width, _device.Viewport.Height).Length();
            for (int x = 0; x < _vertices.Length; x++)
            {

                float weight = 1/(1 + (_vertices[x].Position - new Vector3(origin, 0)).Length());
                weight *= 0.8f + 0.2f * (float)rand.NextDouble();
                weight /= 5;
                fallSpeed[x] = weight;
            }

            BACON = Content.Load<Texture2D>("Bacon");

            _clap = Sounds.GetSoundEffectInstance("Well_Done");
            _clap.IsLooped = true;
            _clap.Volume = 1;
            _fanfare = Sounds.GetSoundEffectInstance("fanfare");
            _fanfare.Play();
            _clap.Play();
        }

        public void Draw(SpriteBatch batch)
        {
            _device.Clear(Color.White);

            batch.Begin();
            batch.Draw(BACON, new Vector2(-120, -120), Color.White);
            batch.Draw(BACON, new Vector2(-70, -70), Color.White);
            batch.Draw(BACON, new Vector2(-20, -20), Color.White);
            batch.Draw(BACON, new Vector2(30, 30), Color.White);
            batch.Draw(BACON, new Vector2(80, 80), Color.White);
            batch.Draw(BACON, new Vector2(130, 130), Color.White);
            batch.DrawString(_font, "You Win!", new Vector2(100, 200), Color.Black);
            batch.DrawString(_font, "Press Space to play again!", new Vector2(100, 280), Color.Black);
            batch.End();

            _device.RasterizerState = RasterizerState.CullNone;
            _effect.CurrentTechnique.Passes[0].Apply();
            _device.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, _vertices, 0, _vertices.Length / 3);
        }

        public void Update(float seconds)
        {
            float diag = new Vector2(_device.Viewport.Width, _device.Viewport.Height).Length();
            for (int x = 0; x < _vertices.Length; x++)
            {
                _vertices[x].Position += this.fallSpeed[x] * Vector3.Forward;
            }

            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Space))
            {
                Manager.Remove(this);
                Manager.Add(new GameScreen());
                _fanfare.Stop();
                _clap.Stop();
            }
        }
    }
}
