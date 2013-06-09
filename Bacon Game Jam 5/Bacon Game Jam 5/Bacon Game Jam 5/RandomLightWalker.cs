using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bacon_Game_Jam_5
{

    class RandomLightWalker:GameObject
    {
        Light light;
        public float Health = 150;
        EnemyMode mode;

        public Vector2 targetPos = Vector2.Zero;
        public float countDown = 0;
        public float speed = 0.5f;
        public bool follow = true;

        public bool collides = true;

        Texture2D _eyes;
        Vector2 _eyeOffset;

        public RandomLightWalker(Vector2 pos, Map map, ContentManager Content):base(map,Content)
        {
            light = map.lightMap.GetLight();
            light.Radius = Health;
            light.Color = Color.White;
            light.Position = pos;
            light.Shadows = true;
            Size = new Vector2(24, 24);
            Position = pos;
            mode = EnemyMode.RandomWalk;
            _eyeOffset = -Size / 2 + new Vector2(1, 0);
            NewTargetPos();
        }

        public override void Move(Vector2 diff)
        {
            if (collides)
                base.Move(diff);
            else
                Position += diff;
        }

        public override void Update(float seconds)
        {
            light.Position = Position;
            light.Radius = Health ;


            switch (mode)
            {
                case EnemyMode.RandomWalk:
                    if ((targetPos - Position).Length() < Map.TileSize || countDown<0)
                        NewTargetPos();
                    else
                    {
                        Vector2 dir = targetPos - Position;
                        dir.Normalize();
                        dir *= speed;
                        Move(new Vector2(dir.X, 0));
                        Move(new Vector2(0, dir.Y));
                        countDown -= seconds;
                    }
                    break;
            }
            base.Update(seconds);
        }

        protected void NewTargetPos()
        {
            targetPos = new Vector2(500, 500) + new Vector2((float)_rand.NextDouble() * 800 - 400, (float)_rand.NextDouble() * 480 - 240);
            countDown = 2*(targetPos - Position).Length() / 60.0f;
        }

        public override void Draw(SpriteBatch batch)
        {
            //batch.Draw(_eyes, Position + _eyeOffset, null, Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0.3f);
        }
    }
}
