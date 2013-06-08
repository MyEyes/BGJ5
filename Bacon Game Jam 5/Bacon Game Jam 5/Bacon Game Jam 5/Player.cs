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
        float Health = 1000;

        public Player(Vector2 pos, Map map, ContentManager Content):base(map,Content)
        {
            light = map.lightMap.GetLight();
            light.Radius = Health / 2.0f;
            light.Color = Color.White;
            Size = new Vector2(5, 5);
            Position = pos;
        }

        public override void Update(float seconds)
        {
            light.Position = Position;
            light.Radius = Health / 2.0f;

            Health -= seconds;
            base.Update(seconds);
        }
    }
}
