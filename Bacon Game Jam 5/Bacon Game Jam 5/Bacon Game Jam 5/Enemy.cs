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
        public float Health = 5000;
        EnemyMode mode;

        Vector2 targetPos = Vector2.Zero;
        float countDown = 0;

        public Enemy(Vector2 pos, Map map, ContentManager Content):base(map,Content)
        {
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

            Player p = _map.Objects[0] as Player;

            if (Health <= 0)
            {
                _map.Objects.Remove(this);
                light.Radius = 0;
                for (int x = 0; x < 40; x++)
                {
                    Vector2 dir = new Vector2((float)(2 * _rand.NextDouble() - 1), (float)(2 * _rand.NextDouble() - 1));
                    dir.Normalize();
                    dir *= 4.5f+(float)_rand.NextDouble();
                    AntiLightParticle alp = new AntiLightParticle(Position, dir, null, _map, null);
                    _map.Objects.Add(alp);
                }
            }

            if (p != null)
            {
                if ((p.Position - Position).Length() < light.Radius/2.0f + p.Health / 4.0f)
                {
                    Vector2 dir = new Vector2((float)(2 * _rand.NextDouble() - 1), (float)(2 * _rand.NextDouble() - 1));
                    dir.Normalize();
                    dir *= 5;
                    LightParticle lp = new LightParticle(p.Position, dir, this, _map, null);
                    _map.Objects.Add(lp);
                    dir = new Vector2((float)(2 * _rand.NextDouble() - 1), (float)(2 * _rand.NextDouble() - 1));
                    dir.Normalize();
                    dir *= 5;
                    AntiLightParticle alp = new AntiLightParticle(Position, dir, p, _map, null);
                    _map.Objects.Add(alp);
                }
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
