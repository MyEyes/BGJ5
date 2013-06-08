using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bacon_Game_Jam_5
{

    enum EnemyMode
    {
        RandomWalk,
        Chase
    }

    class Enemy:GameObject
    {
        Light light;
        float Health = 5000;
        EnemyMode mode;
        static Random _rand;

        Vector2 targetPos = Vector2.Zero;
        float countDown = 0;

        public Enemy(Vector2 pos, Map map, ContentManager Content):base(map,Content)
        {
            if (_rand == null)
                _rand = new Random();

            light = map.lightMap.GetAntiLight();
            light.Radius = Health / 5.0f;
            light.Color = Color.White;
            light.Position = pos;
            Size = new Vector2(5, 5);
            Position = pos;
            mode = EnemyMode.RandomWalk;
            NewTargetPos();
        }

        public override void Update(float seconds)
        {
            light.Position = Position;
            light.Radius = Health / 5.0f;

            Health -= seconds;

            if (Health <= 0)
            {
                _map.Objects.Remove(this);
                light.Radius = 0;
            }

            switch (mode)
            {
                case EnemyMode.RandomWalk:
                    if ((targetPos - Position).Length() < Map.TileSize || countDown<0)
                        NewTargetPos();
                    else
                    {
                        Vector2 dir = targetPos - Position;
                        dir.Normalize();
                        dir /= 2;
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
            targetPos = new Vector2((float)_rand.NextDouble() * Map.SizeX * Map.TileSize, (float)_rand.NextDouble() * Map.SizeY * Map.TileSize);
            countDown = 2*(targetPos - Position).Length() / 60.0f;
        }
    }
}
