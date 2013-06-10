using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Bacon_Game_Jam_5
{
    static class Sounds
    {
        static Dictionary<string, SoundEffect> _sounds;
        static Dictionary<string, SoundEffectInstance[]> _soundInstances;
        static Dictionary<string, int> _lastInstance;
        const int SoundEffectInstances = 30;
        static ContentManager _content;
        static SoundEffectInstance background;

        public static void SetUp(ContentManager Content )
        {
            _sounds = new Dictionary<string, SoundEffect>();
            _soundInstances = new Dictionary<string, SoundEffectInstance[]>();
            _lastInstance = new Dictionary<string, int>();
            _content = Content;
            PlayBackground("Ambience");
        }

        public static void PlayBackground(string name)
        {
            if (background != null)
                background.Dispose();
            SoundEffect effect = _content.Load<SoundEffect>(name);
            background = effect.CreateInstance();
            background.IsLooped = true;
            background.Play();
        }

        public static void PitchBackground(float pitch)
        {
            background.Pitch = pitch;
        }

        public static SoundEffectInstance GetSoundEffectInstance(string name)
        {
            if (_sounds.ContainsKey(name))
            {
                if (_sounds[name] != null)
                    return _sounds[name].CreateInstance();
            }
            else
            {
                try
                {
                    _sounds.Add(name, _content.Load<SoundEffect>(name));

                    if (_sounds[name] != null)
                    {
                        return _sounds[name].CreateInstance();
                    }
                }
                catch (ContentLoadException cle)
                {
                    _sounds.Add(name, null);
                }
            }
            return null;
        }

        public static void PlaySound(string name)
        {
            if (_sounds.ContainsKey(name))
            {
                if (_sounds[name] != null)
                    _sounds[name].Play(1, 0, 0);
            }
            else
            {
                try
                {
                    SoundEffect eff = _content.Load<SoundEffect>(name);
                    _sounds.Add(name, eff);
                    SoundEffectInstance[] instances = new SoundEffectInstance[SoundEffectInstances];
                    for (int x = 0; x < SoundEffectInstances; x++)
                    {
                        instances[x] = eff.CreateInstance();
                    }
                    _soundInstances.Add(name, instances);
                    _lastInstance.Add(name, 0);

                    if (_sounds[name] != null)
                        _sounds[name].Play(1, 0, 0);
                }
                catch (ContentLoadException cle)
                {
                    _sounds.Add(name, null);
                }
            }
        }

        public static void PlaySoundInstance(string name, float pitch=0)
        {
            if (_sounds.ContainsKey(name))
            {
                if (_sounds[name] != null)
                {
                    int lastInstance = _lastInstance[name];
                    lastInstance++;
                    lastInstance %= SoundEffectInstances;
                    _soundInstances[name][lastInstance].Stop(true);
                    _soundInstances[name][lastInstance].Pitch = pitch;
                    _soundInstances[name][lastInstance].Play();
                    _lastInstance[name] = lastInstance;
                }
            }
            else
            {
                try
                {
                    SoundEffect eff = _content.Load<SoundEffect>(name);
                    _sounds.Add(name, eff);
                    SoundEffectInstance[] instances = new SoundEffectInstance[SoundEffectInstances];
                    for (int x = 0; x < SoundEffectInstances; x++)
                    {
                        instances[x] = eff.CreateInstance();
                    }
                    _soundInstances.Add(name, instances);
                    _lastInstance.Add(name, 0);

                    if (_sounds[name] != null)
                        _sounds[name].Play(1, pitch, 0);
                }
                catch (ContentLoadException cle)
                {
                    _sounds.Add(name, null);
                }
            }
        }
    }
}
