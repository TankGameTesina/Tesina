using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProvaXNAGame;

namespace provaXNAGame
{
    public class BaseObject
    {
        public Vector2 Position;
        public Texture2D Texture;
        public Rectangle BoundingBox;
        protected Vector2 origin;
        protected Rectangle sourceRectangle;

        public BaseObject()
        {

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
    }
}
