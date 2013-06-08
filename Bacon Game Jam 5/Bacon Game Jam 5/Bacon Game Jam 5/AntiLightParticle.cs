using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bacon_Game_Jam_5
{
    class AntiLightParticle:GameObject
    {
        Vector2 _direction;
        GameObject _target;
        float countdown;
        Light light;

        public AntiLightParticle(Vector2 position, Vector2 direction, GameObject target, Map map, ContentManager Content)
            : base(map, Content)
        {
            Position = position;
            _direction = direction;
            _target = target;
            countdown = 5;
            light = map.lightMap.GetAntiLight();
            if (light != null)
            {
                light.Radius = 10;
                light.Position = Position;
                light.Color = Color.White;
            }
        }

        public override void Update(float seconds)
        {
            if (light != null)
                light.Position = Position;
            if (countdown > 0)
            {
                countdown -= seconds;
                if (countdown <= 0)
                {
                    Done();
                }
            }
            if (_target != null)
            {
                if ((_target.Position - Position).Length() < Map.TileSize)
                {
                    Player p = _target as Player;
                    if (p != null)
                        p.Health -= 0.5f;
                    Done();
                    
                }
                float speed = _direction.Length();
                Vector2 diff = _target.Position - Position;
                diff.Normalize();
                _direction += diff;
                _direction.Normalize();
                _direction *= speed;
            }
            Position += _direction;
            base.Update(seconds);
        }

        public void Done()
        {
            _map.Objects.Remove(this);
            if (light != null)
                light.Radius = 0;
        }
    }
}
