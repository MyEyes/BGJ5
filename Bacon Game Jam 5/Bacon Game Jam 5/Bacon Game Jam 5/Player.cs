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
        public float Health = 1000;
        public const float MaxHealth = 1000;//Not going to be strictly enforced

        public Player(Vector2 pos, Map map, ContentManager Content):base(map,Content)
        {
            light = map.lightMap.GetLight();
            light.Radius = Health / 2.0f;
            light.Color = Color.White;
            light.Shadows = true;
            Size = new Vector2(8, 8);
            Position = pos;
        }

        public override void Update(float seconds)
        {
            light.Position = Position-Size/2;
            light.Radius = Health / 2.0f;

            Health -= seconds;
            base.Update(seconds);
        }
    }
}
