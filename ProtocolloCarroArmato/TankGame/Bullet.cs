using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class Bullet
    {
        int id;
        int player; //id del giocatore che spara il proiettil
        Posizione pos;

        public Bullet(int id, int player, Posizione pos)
        {
            this.Id = id;
            this.Player = player;
            this.Pos = pos;
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

        public int Player
        {
            get
            {
                return player;
            }

            set
            {
                player = value;
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
    }
}
