using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using provaXNAGame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProvaXNAGame
{
    public class GameObject:BaseObject
    {
        public int Health = 100;
        public float angle;
        public int speed;
        public List<BulletObject> bullets = new List<BulletObject>();
        private Texture2D bullet;
        static public MouseState mouseState;
        static public MouseState previousState;

        public virtual void Init(Vector2 pos, Texture2D text, Texture2D txBullet)
        {
            Random rand = new Random();            

            //coloro il tank
            Color[] data = new Color[text.Width * text.Height];
            text.GetData(data);

            Color colorBase = new Color(0,0,0,0);
            Color color = new Color(Convert.ToByte(rand.Next(255)), Convert.ToByte(rand.Next(255)), Convert.ToByte(rand.Next(255)));

            for (int i = 0; i < data.Length; i++)
                 if (data[i] != colorBase && data[i].A > 50)
                {
                    data[i].R = color.R;
                    data[i].G = color.G;
                    data[i].B = color.B;
                }

            text.SetData(data);

            Position = pos;
            Texture = text;
            bullet = txBullet;
            origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            angle = 0;
            speed = 5;
        }

        public virtual void Update()
        {
            setAngle();
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

            foreach(BulletObject bullet in bullets.Reverse<BulletObject>())
            {
                if (bullet.destroy)
                    bullets.Remove(bullet);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, sourceRectangle, Color.White, angle, origin, 1.0f, SpriteEffects.None, 1);
            for(int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update();
                bullets[i].Draw(spriteBatch);
            }
        }

        public void moveForward()
        {
            float cos = (float)-Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            int x = Convert.ToInt32(Position.X + (speed * sin));
            int y = Convert.ToInt32(Position.Y + (speed * cos));
            Position.X = x;
            Position.Y = y;

        }

        public void moveBack()
        {
            float cos = (float)-Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            int x = Convert.ToInt32(Position.X - (speed * sin));
            int y = Convert.ToInt32(Position.Y - (speed * cos));
            Position.X = x;
            Position.Y = y;
        }

        public void setAngle()
        {
            previousState = mouseState;
            mouseState = Mouse.GetState();

            float mouseX = mouseState.X;
            float mouseY = mouseState.Y;


            double a = Position.X - mouseX;
            double b = Position.Y - mouseY;
            double c = Position.X - Position.X;
            double d = Position.Y - Position.Y;

            double atanA = Math.Atan2(a, b);
            double atanB = Math.Atan2(c, d);

            double risultato = (atanA - atanB) * (-180 / Math.PI);

            if (risultato < 0)
                risultato = 360 + risultato;

            angle = (float)(risultato / (180 / Math.PI));

        }

        public void newBullet()
        {
            BulletObject objBullet = new BulletObject();
            objBullet.Init(Position, bullet, angle);
            bullets.Add(objBullet);
        }

        public void removeBullet(BulletObject bullet)
        {
            bullets.Remove(bullet);
        }
    }
}
