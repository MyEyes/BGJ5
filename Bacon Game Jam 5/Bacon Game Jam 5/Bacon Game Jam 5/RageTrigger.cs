using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bacon_Game_Jam_5
{
    class RageTrigger:GameObject
    {
        Light light;
        float lifeCountdown=0.5f;

        public RageTrigger(Vector2 position, Map map, ContentManager content):base(map,content)
        {
            Position = position;
            light = _map.lightMap.GetLight();
            light.Radius = 50;
            light.Position = position;
            light.Color = Color.Red;
        }

        public override void Update(float seconds)
        {
            Player p = _map.Objects[0] as Player;

            if ((p.Position - Position).Length() < 150)
            {
                Vector2 dir = new Vector2((float)(2 * _rand.NextDouble() - 1), (float)(2 * _rand.NextDouble() - 1));
                dir.Normalize();
                dir *= 5;
                LightParticle light = new LightParticle(Position, dir, p, Color.Red, _map, null);
                lifeCountdown -= seconds;
                _map.Objects.Add(light);
                if (lifeCountdown < 0)
                {
                    p.berserkCountdown = 8;
                    this.light.Radius = 0;
                    _map.Objects.Remove(this);
                }
            }
        }

    }
}
