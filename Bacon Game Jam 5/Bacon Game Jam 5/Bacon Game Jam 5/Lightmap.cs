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
        public bool Shadows=false;

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
        const int MaxLights = 2048;
        Light[] _lights = new Light[MaxLights];

        Light[] _antiLights = new Light[MaxLights];

        GraphicsDevice _device;
        Effect LightEffect;

        RenderTarget2D _lightMap;
        Texture2D _noiseTexture;
        float time = 0;

        BlendState _multiplicative;
        BlendState _subtractive;
        BlendState _noDraw;

        DepthStencilState _generateStencil;
        DepthStencilState _stencilCutout;

        VertexBuffer _VBuffer;
        IndexBuffer _IBuffer;

        VertexBuffer shadowVB;
        IndexBuffer shadowIB;

        public Color AmbientColor;

        public static Matrix rotateRight;
        public static Matrix rotateLeft;

        const int maxX = 30;
        const int maxY = 30;

        public Lightmap(GraphicsDevice device, ContentManager Content)
        {
            rotateLeft = Matrix.CreateRotationZ(0 * MathHelper.Pi / 180);
            rotateRight = Matrix.CreateRotationZ(0 * MathHelper.Pi / 180);
            



            _device = device;
            LightEffect = Content.Load<Effect>("LightEffect");
            _noiseTexture = Content.Load<Texture2D>("noise");
            LightEffect.Parameters["Noise"].SetValue(_noiseTexture);

            _lightMap = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            _VBuffer = new VertexBuffer(device, VertexPositionColorTexture.VertexDeclaration, 2*MaxLights * 4, BufferUsage.None);
            _IBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, 2*MaxLights * 6, BufferUsage.None);
            short[] indices = new short[2*MaxLights * 6];
            for (int x = 0; x < 2*MaxLights * 4; x += 4)
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

            _multiplicative = new BlendState();
            _multiplicative.ColorSourceBlend = Blend.DestinationColor;
            _multiplicative.ColorDestinationBlend = Blend.Zero;

            _subtractive = new BlendState();
            _subtractive.ColorBlendFunction = BlendFunction.ReverseSubtract;
            _subtractive.ColorDestinationBlend = Blend.One;
            _subtractive.ColorSourceBlend = Blend.One;

            shadowIB = new IndexBuffer(device, IndexElementSize.SixteenBits, 4 * 3 * 3 * maxX * maxY, BufferUsage.WriteOnly);
            indices = new short[4 * 3 * 3 * maxX * maxY];
            for (int x = 0; x < 4 * 3 * 3 * maxX * maxY; x += 12)
            {
                short offset = (short)(6 * x / 12);
                indices[x] = (short)(offset);
                indices[x + 1] = (short)(offset + 1);
                indices[x + 2] = (short)(offset + 4);

                indices[x + 3] = (short)(offset + 1);
                indices[x + 4] = (short)(offset + 2);
                indices[x + 5] = (short)(offset + 4);

                indices[x + 6] = (short)(offset + 2);
                indices[x + 7] = (short)(offset + 5);
                indices[x + 8] = (short)(offset + 4);

                indices[x + 9] = (short)(offset + 2);
                indices[x + 10] = (short)(offset + 3);
                indices[x + 11] = (short)(offset + 5);
            }
            shadowIB.SetData<short>(indices);
            shadowVB = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, 6 * 3 * maxX * maxY, BufferUsage.None);

            _generateStencil = new DepthStencilState();
            _generateStencil.DepthBufferWriteEnable = false;
            _generateStencil.StencilPass = StencilOperation.Increment;
            _generateStencil.ReferenceStencil = 1;
            _generateStencil.StencilEnable = true;
            _generateStencil.StencilFunction = CompareFunction.Always;

            _stencilCutout = new DepthStencilState();
            _stencilCutout.DepthBufferEnable = false;
            _stencilCutout.StencilEnable = true;
            _stencilCutout.StencilFunction = CompareFunction.Equal;
            _stencilCutout.ReferenceStencil = 0;

            _noDraw = new BlendState();
            _noDraw.ColorSourceBlend = Blend.Zero;
            _noDraw.ColorWriteChannels = ColorWriteChannels.None;
        }

        public void DrawLights(Camera cam,SpriteBatch batch, Map map)
        {

            _device.SetRenderTarget(_lightMap);

            _device.Clear(AmbientColor);
            time += 0.001f;

            map.Draw(cam, batch, _noDraw);
            DrawShadowedLights(cam, map);
            DrawNormalLights(cam);
            _device.SetRenderTarget(null);
        }

        public void DrawNormalLights(Camera cam)
        {

            _device.SetVertexBuffer(null);
            int offset = 0;
            for (int x = 0; x < MaxLights; x++)
            {
                if (_lights[x] != null && _lights[x].Radius > 0 && !_lights[x].Shadows)
                {
                    _lights[x].UpdateBuffer(offset, _VBuffer);
                    offset += 4;
                }
            }
            int antiOffset = offset;
            for (int x = 0; x < MaxLights; x++)
            {
                if (_antiLights[x] != null && _antiLights[x].Radius > 0 && !_antiLights[x].Shadows)
                {
                    _antiLights[x].UpdateBuffer(antiOffset, _VBuffer);
                    antiOffset += 4;
                }
            }


            _device.DepthStencilState = DepthStencilState.None;
            _device.BlendState = BlendState.Additive;
            _device.RasterizerState = RasterizerState.CullNone;

            LightEffect.Parameters["World"].SetValue(Matrix.Identity);
            LightEffect.Parameters["View"].SetValue(cam.ViewMatrix);
            LightEffect.Parameters["Projection"].SetValue(cam.ProjectionMatrix);
            LightEffect.Parameters["time"].SetValue(-time);

            LightEffect.CurrentTechnique = LightEffect.Techniques["DrawLight"];

            _device.SetVertexBuffer(_VBuffer);
            _device.Indices = _IBuffer;
            LightEffect.CurrentTechnique.Passes[0].Apply();
            if (offset > 0)
                _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, offset, 0, offset / 2);

            _device.BlendState = _subtractive;
            LightEffect.Parameters["time"].SetValue(time);
            LightEffect.CurrentTechnique.Passes[0].Apply();
            if (antiOffset - offset > 0)
                _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, antiOffset - offset, 3 * offset / 2, (antiOffset - offset) / 2);
        }

        public void DrawShadowedLights(Camera cam, Map map)
        {

            _device.DepthStencilState = DepthStencilState.None;
            _device.RasterizerState = RasterizerState.CullNone;

            LightEffect.Parameters["World"].SetValue(Matrix.Identity);
            LightEffect.Parameters["View"].SetValue(cam.ViewMatrix);
            LightEffect.Parameters["Projection"].SetValue(cam.ProjectionMatrix);
            LightEffect.Parameters["time"].SetValue(-time);

            for (int x = 0; x < MaxLights; x++)
            {
                if (_lights[x] != null && _lights[x].Radius > 0 && _lights[x].Shadows)
                {
                    _device.Clear(ClearOptions.Stencil, Color.White, 0, 0);

                    _device.DepthStencilState = _generateStencil;
                    _device.BlendState = _noDraw;
                    LightEffect.CurrentTechnique = LightEffect.Techniques["Shadow"];
                    int vertices = map.CreateShadowGeometry(_lights[x].Position, new Rectangle((int)(_lights[x].Position.X - _lights[x].Radius), (int)(_lights[x].Position.Y - _lights[x].Radius), (int)(2 * _lights[x].Radius), (int)(2 * _lights[x].Radius)), shadowVB);

                    
                    VertexPositionColor[] test = new VertexPositionColor[shadowVB.VertexCount];
                    shadowVB.GetData<VertexPositionColor>(test);
                     
                    test = new VertexPositionColor[]
                    {
                        new VertexPositionColor(new Vector3(44,323,0),Color.White),
                        new VertexPositionColor(new Vector3(32,224,0),Color.White),
                        new VertexPositionColor(new Vector3(32,256,0),Color.White),
                        new VertexPositionColor(new Vector3(42,355,0),Color.White),
                        new VertexPositionColor(new Vector3(32,224,0),Color.White),
                        new VertexPositionColor(new Vector3(32,256,0),Color.White),
                    };
                    shadowVB.SetData<VertexPositionColor>(test, 0, 6);


                    _device.SetVertexBuffer(shadowVB);
                    _device.Indices = shadowIB;
                    LightEffect.CurrentTechnique.Passes[0].Apply();
                    if (vertices > 0)
                        _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices, 0, 4 * vertices / 6);

                    _lights[x].UpdateBuffer(0, _VBuffer);
                    _device.DepthStencilState = _stencilCutout;
                    _device.BlendState = BlendState.Additive;
                    _device.SetVertexBuffer(_VBuffer);
                    _device.Indices = _IBuffer;
                    LightEffect.CurrentTechnique = LightEffect.Techniques["DrawLight"];
                    LightEffect.CurrentTechnique.Passes[0].Apply();
                    _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
                    
                }
            }

        }

        public void DrawLightmap(SpriteBatch batch)
        {
            batch.Begin(SpriteSortMode.Immediate, _multiplicative, null, DepthStencilState.DepthRead, null);
            batch.Draw(_lightMap, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.4f);
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

        public Light GetAntiLight()
        {
            for (int x = 0; x < MaxLights; x++)
            {
                if (_antiLights[x] == null || _antiLights[x].Radius == 0)
                {
                    _antiLights[x] = new Light();
                    return _antiLights[x];
                }
            }
            return null;
        }
    }
}
