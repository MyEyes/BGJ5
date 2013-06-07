using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bacon_Game_Jam_5
{
    public class Light
    {
        public Vector2 Position;
        public float Radius;
        public Color Color;

        public void UpdateBuffer(int offset, VertexBuffer buffer)
        {
            VertexPositionColorTexture[] vpct = new VertexPositionColorTexture[4];
            vpct[0] = new VertexPositionColorTexture(new Vector3(Position.X - Radius, Position.Y - Radius, 0), Color, new Vector2(0, 0));
            vpct[1] = new VertexPositionColorTexture(new Vector3(Position.X + Radius, Position.Y - Radius, 0), Color, new Vector2(1, 0));
            vpct[2] = new VertexPositionColorTexture(new Vector3(Position.X + Radius, Position.Y + Radius, 0), Color, new Vector2(1, 1));
            vpct[3] = new VertexPositionColorTexture(new Vector3(Position.X - Radius, Position.Y + Radius, 0), Color, new Vector2(0, 1));
            buffer.SetData<VertexPositionColorTexture>(offset * VertexPositionColorTexture.VertexDeclaration.VertexStride, vpct, 0, 4, VertexPositionColorTexture.VertexDeclaration.VertexStride);
        }
    }

    public class Lightmap
    {
        const int MaxLights = 60;
        Light[] _lights = new Light[MaxLights];

        GraphicsDevice _device;
        Effect LightEffect;

        RenderTarget2D _lightMap;

        VertexBuffer _VBuffer;
        IndexBuffer _IBuffer;

        public Lightmap(GraphicsDevice device, ContentManager Content)
        {
            _device = device;
            LightEffect = Content.Load<Effect>("LightEffect");

            _lightMap = new RenderTarget2D(device, device.DisplayMode.Width, device.DisplayMode.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            _VBuffer = new VertexBuffer(device, VertexPositionColorTexture.VertexDeclaration, MaxLights * 4, BufferUsage.None);
            _IBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, MaxLights * 6, BufferUsage.None);
            short[] indices = new short[MaxLights * 6];
            for (int x = 0; x < MaxLights * 4; x += 4)
            {
                int indexOffset = 6 * x / 4;
                indices[indexOffset] = (short)x;
                indices[indexOffset + 1] = (short)(x + 1);
                indices[indexOffset + 2] = (short)(x + 2);
                indices[indexOffset + 3] = (short)x;
                indices[indexOffset + 4] = (short)(x + 2);
                indices[indexOffset + 5] = (short)(x + 3);
            }
            _IBuffer.SetData<short>(indices);
        }

        public void DrawLights(Camera cam)
        {
            int offset = 0;
            for (int x = 0; x < MaxLights; x++)
            {
                if (_lights[x] != null && _lights[x].Radius>0)
                {
                    _lights[x].UpdateBuffer(offset, _VBuffer);
                    offset += 4;
                }
            }
            VertexPositionColorTexture[] test = new VertexPositionColorTexture[MaxLights * 4];
            _VBuffer.GetData<VertexPositionColorTexture>(test);

            _device.SetRenderTarget(_lightMap);

            _device.Clear(Color.Black);
            _device.DepthStencilState = DepthStencilState.None;
            _device.BlendState = BlendState.Additive;
            _device.RasterizerState = RasterizerState.CullNone;

            LightEffect.Parameters["World"].SetValue(Matrix.Identity);
            LightEffect.Parameters["View"].SetValue(cam.ViewMatrix);
            LightEffect.Parameters["Projection"].SetValue(cam.ProjectionMatrix);

            LightEffect.CurrentTechnique.Passes[0].Apply();
            _device.SetVertexBuffer(_VBuffer);
            _device.Indices = _IBuffer;
            if (offset > 0)
                _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, offset, 0, offset / 2);

            _device.SetRenderTarget(null);
        }

        public void DrawLightmap(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(_lightMap, Vector2.Zero, Color.White);
            batch.End();
        }

        public Light GetLight()
        {
            for (int x = 0; x < MaxLights; x++)
            {
                if (_lights[x] == null || _lights[x].Radius == 0)
                {
                    _lights[x] = new Light();
                    return _lights[x];
                }
            }
            return null;
        }
    }
}
