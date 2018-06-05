using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace provaXNAGame
{
    public class BaseObject
    {
        private Vector2 position;
        private Vector2 lastPosition;
        public Texture2D Texture;
        public Rectangle BoundingBox;
        public float angle;
        protected Vector2 origin;
        protected Rectangle sourceRectangle;

        public Vector2 Position
        {
            get
            {
                return position;
            }

            set
            {
                lastPosition = position;
                position = value;

            }
        }

        public Vector2 LastPosition
        {
            get
            {
                return lastPosition;
            }

            set
            {
                lastPosition = value;
            }
        }

        public bool Colliding(GameObject obj)
        {
            bool col = false;

            if (BoundingBox.Intersects(obj.BoundingBox))
            {
                col = true;
            }

            return col;
        }

        public override string ToString()
        {
            return Position.X+";"+Position.Y + ";" + angle;
        }
    }
}
