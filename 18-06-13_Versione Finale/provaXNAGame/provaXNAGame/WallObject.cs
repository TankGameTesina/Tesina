using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace provaXNAGame
{
    public class WallObject:BaseObject
    {
        public bool destroy;

        public virtual void Init(Vector2 pos, Texture2D text)
        {
            Position = pos;
            Texture = text;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            SourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
        }

        public virtual void Update()
        {
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, SourceRectangle, Color.White, 0, Origin, 1.0f, SpriteEffects.None, 1);
        }
    }
}
