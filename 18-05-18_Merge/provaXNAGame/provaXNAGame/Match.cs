using provaXNAGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clientServer
{
    public class Match
    {
        private List<GameObject> players;
        private GameObject winner;

        public Match(List<GameObject> players, GameObject winner)
        {
            this.Players = players;
            this.Winner = winner;
        }

        internal List<GameObject> Players
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

        internal GameObject Winner
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
