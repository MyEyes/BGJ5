using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bacon_Game_Jam_5
{
    interface IScreen
    {
        void Initialize(GraphicsDevice device, ContentManager Content);
        void Update(float seconds);
        void Draw(SpriteBatch batch);

        ScreenManager Manager { get; set; }
    }
}
