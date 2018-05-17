using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace provaXNAGame
{
    public class BaseObject
    {
        public Vector2 Position;
        public Texture2D Texture;
        public Rectangle BoundingBox;
        protected Vector2 origin;
        protected Rectangle sourceRectangle;

        public bool Colliding(BaseObject obj)
        {
            if (BoundingBox.Intersects(obj.BoundingBox))
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return Position.X + ";" + Position.Y + ";0";
        }
    }
}
