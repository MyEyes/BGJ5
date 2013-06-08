using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bacon_Game_Jam_5
{
    public class GameObject
    {
        public Vector2 Position;
        public Vector2 Size;

        protected Map _map;

        protected static Random _rand = new Random();

        public GameObject(Map map, ContentManager Content)
        {
            _map = map;
        }

        public virtual void Move(Vector2 diff)
        {
            Position += diff;
            if (_map.Collides(this))
                Position -= diff;
        }

        public virtual void Update(float seconds)
        {

        }

        public virtual void Draw(SpriteBatch batch)
        {

        }

        public Rectangle BoundingRect
        {
            get { return new Rectangle((int)(Position.X - Size.X / 2), (int)(Position.Y - Size.Y / 2), (int)Size.X, (int)Size.Y); }
        }
    }
}
