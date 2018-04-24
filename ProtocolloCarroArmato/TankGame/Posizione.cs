using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class Posizione
    {
        private int x;
        private int y;
        private int z;

        public Posizione()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        public Posizione(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public int X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public int Z
        {
            get
            {
                return z;
            }

            set
            {
                z = value;
            }
        }

        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static Posizione operator + (Posizione start, int move)
        {
            Posizione newPos = new Posizione();

            double cos = -Math.Cos(DegreeToRadian(start.z));
            double sin = Math.Sin(DegreeToRadian(start.z));

            newPos.X = Convert.ToInt32(start.X + (move * sin));
            newPos.Y = Convert.ToInt32(start.Y + (move * cos));
            newPos.Z = start.Z;
            
            return newPos;
        }

        public static Posizione operator - (Posizione start, int move)
        {
            Posizione newPos = new Posizione();

            double cos = -Math.Cos(DegreeToRadian(start.z));
            double sin = Math.Sin(DegreeToRadian(start.z));

            newPos.X = Convert.ToInt32(start.X - (move * sin));
            newPos.Y = Convert.ToInt32(start.Y - (move * cos));
            newPos.Z = start.Z;

            return newPos;
        }

        override public string ToString()
        {
            return X + ";" + Y + ";" + Z;
        }
    }
}
