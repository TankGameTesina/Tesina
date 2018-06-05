using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace provaXNAGame
{
    public class BaseObject
    {
        protected Vector2 position;
        protected Vector2 lastPosition;
        private Texture2D texture;
        protected Rectangle boundingBox;
        public int Id;
        private float angle;
        private int speed;
        private Vector2 origin;
        private Rectangle sourceRectangle;

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

        public Rectangle BoundingBox
        {
            get
            {
                return boundingBox;
            }

            set
            {
                boundingBox = value;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return texture;
            }

            set
            {
                texture = value;
            }
        }

        public float Angle
        {
            get
            {
                return angle;
            }

            set
            {
                angle = value;
            }
        }

        public int Speed
        {
            get
            {
                return speed;
            }

            set
            {
                speed = value;
            }
        }

        public Vector2 Origin
        {
            get
            {
                return origin;
            }

            set
            {
                origin = value;
            }
        }

        public Rectangle SourceRectangle
        {
            get
            {
                return sourceRectangle;
            }

            set
            {
                sourceRectangle = value;
            }
        }

        public BaseObject()
        {

        }

        public BaseObject(Rectangle posizione, Vector2 pos)
        {
            BoundingBox = posizione;
            Position = pos;
        }

        public BaseObject(Rectangle posizione, Vector2 pos, int id)
        {
            BoundingBox = posizione;
            Position = pos;
            Id = id;
        }

        public int getXMovement(string forbck)
        {
            float sin = (float)Math.Sin(Angle);

            switch (forbck)
            {
                case "f":
                    return Convert.ToInt32(Position.X + (Speed * sin));
                case "b":
                    return Convert.ToInt32(Position.X - (Speed * sin));
                default:
                    return 0;
            }
        }

        public int getYMovement(string forbck)
        {
            float cos = (float)-Math.Cos(Angle);

            switch (forbck)
            {
                case "f":
                    return Convert.ToInt32(Position.Y + (Speed * cos));
                case "b":
                    return Convert.ToInt32(Position.Y - (Speed * cos));
                default:
                    return 0;
            }
        }

        public bool TankColliding(BaseObject obj)
        {
            if (BoundingBox.Intersects(obj.BoundingBox))
            {
                return true;
            }

            return false;
        }

        public bool WallColliding(BaseObject muro)
        {
            if ((Position.X + BoundingBox.Height >= muro.Position.X && Position.X <= muro.Position.X + muro.BoundingBox.Width) &&
                (Position.Y <= muro.Position.Y + muro.BoundingBox.Height && Position.Y + BoundingBox.Height >= muro.Position.Y))
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return Position.X+";"+Position.Y + ";" + Angle;
        }

        public BaseObject PosizioneFutura(string move)
        {
            return new BaseObject(new Rectangle(Convert.ToInt16(this.getXMovement(move)), Convert.ToInt16(this.getYMovement(move)), this.Texture.Width, this.Texture.Height), new Vector2(Convert.ToInt16(this.getXMovement(move)), Convert.ToInt16(this.getYMovement(move))), this.Id);
        }
    }
}
