using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bacon_Game_Jam_5
{
    class Player : GameObject
    {
        Light light;
        public float Health = 100;
        public const float MaxHealth = 1000;//Not going to be strictly enforced

        Texture2D _eyes;

        public float berserkCountdown = 0;
        Vector2 _eyeOffset;

        int particleCounter = 0;

        SoundEffectInstance _heartbeat;
        SoundEffectInstance _berserk;
        bool heartBeatPlaying = false;
        bool berserkPlaying = false;
        

        public Player(Vector2 pos, Map map, ContentManager Content):base(map,Content)
        {
            light = map.lightMap.GetLight();
            light.Radius = Health / 2.0f;
            light.Color = Color.White;
            light.Shadows = true;
            Size = new Vector2(24, 24);
            _eyeOffset = -Size / 2 + new Vector2(1, 4);
            Position = pos;
            _eyes = Content.Load<Texture2D>("Eyes");

            _heartbeat = Sounds.GetSoundEffectInstance("Heartbeat");
            _heartbeat.IsLooped = true;
            _berserk = Sounds.GetSoundEffectInstance("Berserk");
            _berserk.IsLooped = true;
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

            if (Health < 200)
            {
                float volume=(200 - Health) / 200.0f;
                volume = volume < 0 ? 0 : volume > 1 ? 1 : volume;
                _heartbeat.Volume = volume;
                if (!heartBeatPlaying)
                {
                    _heartbeat.Play();
                    heartBeatPlaying = true;
                }
            }
            else if(heartBeatPlaying)
            {
                _heartbeat.Stop();
                heartBeatPlaying = false;
            }

            if (berserkCountdown > 0)
            {
                if (berserkCountdown < 1)
                    _berserk.Volume = berserkCountdown;
                if (!berserkPlaying)
                {
                    _berserk.Volume = 1;
                    _berserk.Play();
                    berserkPlaying = true;
                }
            }
            else if (berserkPlaying)
            {
                _berserk.Stop();
                berserkPlaying = false;
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
            LightParticle lp = new LightParticle(Position, dir, e, berserkCountdown > 0 ? Color.Red : Color.White, _map, null, particleCounter % 8 == 0);
            _map.Objects.Add(lp);

            particleCounter++;

            if (berserkCountdown > 0)
            {
                dir *= -1.25f;
                lp = new LightParticle(Position, dir, e, berserkCountdown > 0 ? Color.Red : Color.White, _map, null, false);
                _map.Objects.Add(lp);
            }
        }

        public override void Draw(SpriteBatch batch)
        {
            //batch.Draw(_eyes, Position + _eyeOffset, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None,0.5f);
        }
    }
}
