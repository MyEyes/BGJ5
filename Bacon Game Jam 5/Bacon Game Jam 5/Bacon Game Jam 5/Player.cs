using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bacon_Game_Jam_5
{
    class Player : GameObject
    {
        Light light;
        public float Health = 100;
        public const float MaxHealth = 1000;//Not going to be strictly enforced

        public float berserkCountdown = 0;

        public Player(Vector2 pos, Map map, ContentManager Content):base(map,Content)
        {
            light = map.lightMap.GetLight();
            light.Radius = Health / 2.0f;
            light.Color = Color.White;
            light.Shadows = true;
            Size = new Vector2(24, 24);
            Position = pos;
        }

        public override void Update(float seconds)
        {
            light.Position = Position;
            light.Radius = Health / 2.0f;
            light.Color = Color.White;
            
            Health -= seconds;

            if (berserkCountdown > 0)
            {
                light.Color = Color.Red;
                berserkCountdown -= seconds;
                if (berserkCountdown < 0)
                    if (Health > MaxHealth)
                        Health = MaxHealth;
            }

            if (Health < 0)
            {
                _map.Objects.Remove(this);
                light.Radius = 0;
            }


            base.Update(seconds);
        }

        public void Attack(Enemy e)
        {
            Vector2 dir = new Vector2((float)(2 * _rand.NextDouble() - 1), (float)(2 * _rand.NextDouble() - 1));
            dir.Normalize();
            dir *= 5;
            LightParticle lp = new LightParticle(Position, dir, e, berserkCountdown > 0 ? Color.Red : Color.White, _map, null);
            _map.Objects.Add(lp);

            if (berserkCountdown > 0)
            {
                dir *= -1.25f;
                lp = new LightParticle(Position, dir, e, berserkCountdown > 0 ? Color.Red : Color.White, _map, null);
                _map.Objects.Add(lp);
            }
        }
    }
}
