using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace provaXNAGame
{
    class myTimer : Timer
    {
        BulletObject blt;

        public myTimer(BulletObject blt, int interval)
        {
            base.Interval = interval;
            this.Blt = blt;
        }

        public BulletObject Blt
        {
            get
            {
                return blt;
            }

            set
            {
                blt = value;
            }
        }
    }
}
