using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bacon_Game_Jam_5
{
    class ScreenManager
    {
        GraphicsDevice _device;
        ContentManager _content;
        List<IScreen> _screens;

        public ScreenManager(GraphicsDevice device, ContentManager content)
        {
            _screens = new List<IScreen>();
            _device = device;
            _content = content;
        }

        public void Add(IScreen screen)
        {
            screen.Manager = this;
            screen.Initialize(_device, _content);
            _screens.Add(screen);
        }

        public void Remove(IScreen screen)
        {
            _screens.Remove(screen);
        }

        public void Update(float seconds)
        {
            for (int x = 0; x < _screens.Count; x++)
            {
                _screens[x].Update(seconds);
            }
        }

        public void Draw(SpriteBatch batch)
        {
            for (int x = 0; x < _screens.Count; x++)
            {
                _screens[x].Draw(batch);
            }
        }
    }
}
