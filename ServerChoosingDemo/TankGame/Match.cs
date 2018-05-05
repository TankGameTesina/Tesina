using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class Match
    {
        private List<Tank> players;
        private Tank winner;

        public Match(List<Tank> players, Tank winner)
        {
            this.Players = players;
            this.Winner = winner;
        }

        internal List<Tank> Players
        {
            get
            {
                return players;
            }

            set
            {
                players = value;
            }
        }

        internal Tank Winner
        {
            get
            {
                return winner;
            }

            set
            {
                winner = value;
            }
        }
    }
}
