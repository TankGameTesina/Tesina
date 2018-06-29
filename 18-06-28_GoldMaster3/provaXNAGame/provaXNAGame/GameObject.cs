using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace provaXNAGame
{
    public class GameObject:BaseObject 
    {
        public string Address;
        public string Username;
        public int Health = 100;
        static public MouseState mouseState;
        static public MouseState previousState;
        public bool inizializzato;

        public int caricatore = 5;
        public bool ricaricando;
        System.Windows.Forms.Timer lTimer;
        public int tempRicarica = 5;

        public GameObject()
        {

        }

        public GameObject(Rectangle posizione, Vector2 pos) : base(posizione, pos)
        {

        }

        public GameObject(Vector2 pos, string username, string address)
        {
            Position = pos;
            Username = username;
            Address = address;
        }

        public bool Fire
        {
            get
            {
                if(ricaricando == false && caricatore > 0)
                {
                    caricatore--;
                    return true;                    
                }

                return false;
            }
        }

        public void Ricarica()
        {
            if (caricatore == 0)
            {
                ricaricando = true;
                lTimer = new System.Windows.Forms.Timer();
                lTimer.Interval = 1000;
                lTimer.Tick += new EventHandler(tmrServer_Tick);
                lTimer.Start();
                ricaricando = true;               
            }
        }

        private void tmrServer_Tick(object sender, EventArgs e)
        {
            if (tempRicarica > 0)
                tempRicarica--;
            else
            {
                caricatore = 5;
                ricaricando = false;
                tempRicarica = 5;
                lTimer.Dispose();
            }
        }
    
        public virtual void Init(Vector2 pos, Texture2D text, string username,Color colore, string address, int id)
        {
            //coloro il tank
            Texture = text;
            SetColor(colore);
            Position = pos;
            LastPosition = Position;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            SourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Username = username;
            Angle = 0;
            Speed = 5;
            Id = id;
            Address = address;
            inizializzato = true;
        }

        public virtual void InitEnemy(Vector2 pos, Texture2D text, string username, string address, int id)
        {
            Position = pos;
            LastPosition = Position;
            Texture = text;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            SourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Angle = 0;
            Speed = 5;
            Id = id;
            Username = username;
            Address = address;
            inizializzato = true;
        }

        public virtual void Update()
        {
            if(inizializzato)
            {
                BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (inizializzato)
            {
                spriteBatch.Draw(Texture, Position, SourceRectangle, Color.White, Angle, Origin, 1.0f, SpriteEffects.None, 1);
            }
        }

        public void SetColor(Color colore)
        {
            Color[] data = new Color[Texture.Width * Texture.Height];
            Texture.GetData(data);
            Color colorBase = new Color(0, 0, 0, 0);
            Color color;

            if (colore == new Color() || colore == null)
            {
                Random rand = new Random();

                color = new Color(Convert.ToByte(rand.Next(255)), Convert.ToByte(rand.Next(255)), Convert.ToByte(rand.Next(255)));
            }
            else
                color = colore;

            for (int i = 0; i < data.Length; i++)
                if (data[i] != colorBase && data[i].A > 50)
                {
                    data[i].R = color.R;
                    data[i].G = color.G;
                    data[i].B = color.B;
                }

            Texture.SetData(data);
        }

        public Color GetColor()
        {
            Color[] data = new Color[Texture.Width * Texture.Height];
            Texture.GetData(data);
            Color colorBase = new Color(0, 0, 0, 0);
            Color coloreRitorno = new Color();

            for (int i = 0; i < data.Length; i++)
                if (data[i] != colorBase && data[i].A > 50)
                {
                    coloreRitorno.R = data[i].R;
                    coloreRitorno.G = data[i].G;
                    coloreRitorno.B = data[i].B;
                    break;
                }

            return coloreRitorno;
        }

        public void moveForward()
        {
            position.X = getXMovement("f");
            position.Y = getYMovement("f");
        }

        public void moveBack()
        {
            position.X = getXMovement("b");
            position.Y = getYMovement("b");
        }

        public float GetAngle()
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

            return (float)(risultato / (180 / Math.PI));
        }
    }
}
