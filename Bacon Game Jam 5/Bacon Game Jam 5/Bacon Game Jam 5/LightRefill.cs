using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bacon_Game_Jam_5
{
    class LightRefill:GameObject
    {

        public float RefillLeft = 0;
        float coolDown = 0;
        const float coolDownTime = 0.3f;

        Light light;

        public LightRefill(Vector2 Position, Map map, ContentManager Content)
            : base(map, Content)
        {
            this.Position = Position;
            RefillLeft = 300;
            light = _map.lightMap.GetLight();
            if (light != null)
            {
                light.Position = Position;
                light.Radius = RefillLeft / 5.0f;
                light.Color = Color.White;
            }
            
        }


        public override void Update(float seconds)
        {
            Player p = _map.Objects[0] as Player;
            coolDown -= seconds;
            light.Radius = RefillLeft / 5.0f;
            if (p != null)
            {
                if ((p.Position - Position).Length() < 300 && coolDown<=0 && p.Health<Player.MaxHealth)
                {
                    Vector2 dir = new Vector2((float)(2 * _rand.NextDouble() - 1), (float)(2 * _rand.NextDouble() - 1));
                    dir.Normalize();
                    dir *= 5;
                    LightParticle lp = new LightParticle(Position, dir, p,Color.White, _map, null,true);
                    _map.Objects.Add(lp);
                    RefillLeft -= 10;
                    coolDown = coolDownTime;
                }
            }
            if (RefillLeft <= 0)
            {
                _map.Objects.Remove(this);
                light.Radius = 0;
                _map.Objects.Add(new LightRefill(new Vector2((float)_rand.NextDouble() * Map.SizeX * Map.TileSize, (float)_rand.NextDouble() * Map.SizeY * Map.TileSize), _map, null));
            }
            base.Update(seconds);
        }
    }
}
