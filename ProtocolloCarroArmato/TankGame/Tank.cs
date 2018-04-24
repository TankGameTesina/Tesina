using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class Tank
    {
        private string user;
        private string color;
        private string imgPath;
        private int id;
        private string address;
        private Posizione pos;

        public string User
        {
            get
            {
                return user;
            }

            set
            {
                user = value;
            }
        }

        public string Color
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

        public string ImgPath
        {
            get
            {
                return imgPath;
            }

            set
            {
                imgPath = value;
            }
        }

        public Posizione Pos
        {
            get
            {
                return pos;
            }

            set
            {
                pos = value;
            }
        }

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Address
        {
            get
            {
                return address;
            }

            set
            {
                address = value;
            }
        }

        public Tank(string user, string color, string imgPath, int iD, Posizione pos)
        {
            this.User = user;
            this.Color = color;
            this.ImgPath = imgPath;
            Id = iD;
            this.Pos = pos;
        }

        public Tank(string user, string color, string imgPath, int iD, string address, Posizione pos)
        {
            this.User = user;
            this.Color = color;
            this.ImgPath = imgPath;
            Id = iD;
            this.Pos = null;
            this.Address = address;
            this.pos = pos;
        }

        public Tank(string user)
        {
            this.User = user;
        }
    }
}
