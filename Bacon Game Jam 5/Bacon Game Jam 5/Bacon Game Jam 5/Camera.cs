using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bacon_Game_Jam_5
{
    public class Camera
    {
        Vector2 _position;
        Rectangle _screenSize;

        public Camera(Vector2 position, Rectangle screenSize)
        {
            _position = position;
            _screenSize = screenSize;
        }

        public void SetPosition(Vector2 position)
        {
            _position = position;
        }

        public void Move(Vector2 diff)
        {
            _position += diff;
        }

        public Matrix ViewMatrix
        {
            get { return Matrix.CreateTranslation(-_position.X+_screenSize.Width/2, -_position.Y+_screenSize.Height/2, 0); }
        }

        public Matrix ProjectionMatrix
        {
            get
            {
                Matrix projection = Matrix.CreateTranslation(-_screenSize.Width / 2 - 0.5f, -_screenSize.Height / 2 - 0.5f, 0) * Matrix.CreateScale(2 / ((float)_screenSize.Width), -2 / ((float)_screenSize.Height), 1);
                return projection;
            }
        }

        public Rectangle ViewSpace
        {
            get { return new Rectangle((int)_position.X - _screenSize.Width / 2 - 1, (int)_position.Y - _screenSize.Height / 2 - 1, _screenSize.Width + 2, _screenSize.Height + 2); }
        }
    }
}
