using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace provaXNAGame
{
    public class BulletObject:BaseObject
    {
        private DateTime dateCreated;
        private DateTime dateUpdate = new DateTime();
        private DateTime dateNow = new DateTime();
        private int idTank;
        public bool destroy;
        public bool inizializzato = false;

        public int IdTank
        {
            get { return idTank; }
            set { idTank = value; }
        }

        public BulletObject()
        {

        }

        public BulletObject(Rectangle posizione, Vector2 pos) : base(posizione, pos)
        {

        }

        public BulletObject(Vector2 pos, float intangle, int id, int tnkId)
        {
            Position = pos;
            Angle = intangle;
            Id = id;
            IdTank = tnkId;
        }

        public virtual void Init(Vector2 pos, Texture2D text, float intangle, int id, int tnkId)
        {
            Position = pos;
            Texture = text;
            Angle = intangle;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            SourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Speed = 10;
            dateCreated = new DateTime();
            dateCreated = DateTime.Now;
            Id = id;
            IdTank = tnkId;
            inizializzato = true;
        }

        public virtual bool Update()
        {
            dateNow = DateTime.Now;

            if (dateNow > dateUpdate.AddMilliseconds(10))
            {
                dateUpdate = DateTime.Now;
                BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, 10, 10);
                move();
                if (dateNow.Second > dateCreated.Second + 5)
                    destroy = true;

                return true;
            }
            return false;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, SourceRectangle, Color.White, Angle, Origin, 1.0f, SpriteEffects.None, 1);
        }

        public void move()
        {
            position.X = getXMovement("f");
            position.Y = getYMovement("f");
        }
    }
}
