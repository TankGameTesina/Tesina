using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class Pacchetto
    {
        private string msg;
        private string address;

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

        public Pacchetto(string msg, string address)
        {
            this.Msg = msg;
            this.Address = address;
        }

        public override string ToString()
        {
            return "indirizzo " + Address + " contiene " + Msg;
        }

    }
}
