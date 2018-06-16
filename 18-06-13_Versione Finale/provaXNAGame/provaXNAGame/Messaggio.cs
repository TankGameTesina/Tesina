using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace provaXNAGame
{
    class Messaggio
    {
        string msg;
        Color color;
        int lifeTime;

        public string Msg
        {
            get
            {
                return msg;
            }

            set
            {
                msg = value;
            }
        }

        public int LifeTime
        {
            get
            {
                return lifeTime;
            }

            set
            {
                lifeTime = value;
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
            }
        }

        public Messaggio(string msg, int lifeTime)
        {
            this.Msg = msg;
            this.LifeTime = lifeTime;
        }

        public Messaggio(string msg, Color color)
        {
            this.Msg = msg;
            LifeTime = 0;
            this.Color = color;
        }
    }
}
