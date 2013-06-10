using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bacon_Game_Jam_5
{
    class LightParticle:GameObject
    {
        Vector2 _direction;
        GameObject _target;
        float countdown;
        Light light;
        Color _color;
        bool _sound;

        public LightParticle(Vector2 position, Vector2 direction, GameObject target, Color color, Map map, ContentManager Content, bool sound=false)
            : base(map, Content)
        {
            Position = position;
            _direction = direction;
            _target = target;
            countdown = 5;
            light = map.lightMap.GetLight();
            _color = color;
            if (light != null)
            {
                light.Radius = 10;
                light.Position = Position;
                light.Color = color;
            }
            _sound = sound;
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
                    int damage = 0;
                    if(_color==Color.White)
                            damage = 5;
                    if (_color == Color.Red)
                        damage = 10;
                    Enemy e = _target as Enemy;
                    if (e != null && e.Health>0)
                    {
                        e.Health -= damage;
                        if (_sound)
                        {
                            Sounds.PlaySoundInstance("hit1",-1);
                        }
                        Done();
                    }
                    Player p = _target as Player;
                    if (p != null)
                    {
                        p.Health += damage;
                        if (_sound)
                        {
                            Sounds.PlaySoundInstance("beep");
                        }
                        Done();
                    }
                }
                float speed = _direction.Length();
                Vector2 diff = _target.Position-Position;
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
