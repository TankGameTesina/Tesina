using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTesinaTrasparenza
{
    class Bullet
    {
        public Point posIniziale;
        public int distanza;
        public double grade;

        public Bullet(Point posIniziale, int distanza, double grade)
        {
            this.posIniziale = posIniziale;
            this.distanza = distanza;
            this.grade = grade;
        }
    }
}
