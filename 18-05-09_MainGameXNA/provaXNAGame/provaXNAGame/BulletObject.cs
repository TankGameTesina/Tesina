using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProvaXNAGame;
using System;
using System.Collections.Generic;

namespace provaXNAGame
{
    public class BulletObject:BaseObject
    {
        public float angle;
        public int speed; private DateTime dateCreated;
        private DateTime dateNow;
        public bool destroy;
        private List<Texture2D> bullets = new List<Texture2D>();

        public virtual void Init(Vector2 pos, Texture2D text, float intangle)
        {
            Position = pos;
            Texture = text;
            angle = intangle;
            origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            speed = 10;
            dateCreated = new DateTime();
            dateCreated = DateTime.Now;
        }

        public virtual void Update()
        {
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, 10, 10);
            move();
            dateNow = new DateTime();
            dateNow = DateTime.Now;
            if (dateNow.Second > dateCreated.Second + 5)
                destroy = true;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, sourceRectangle, Color.White, angle, origin, 1.0f, SpriteEffects.None, 1);
        }

        public void move()
        {
            float cos = (float)-Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            int x = Convert.ToInt32(Position.X + (speed * sin));
            int y = Convert.ToInt32(Position.Y + (speed * cos));
            Position.X = x;
            Position.Y = y;

        }
    }
}
