using SpriteLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainGame
{
    class Bullet
    {
        public Point posIniziale;
        public Sprite colpo;
        public int distanza;
        public double grade;

        public Bullet(Point posIniziale, int distanza, double grade, Sprite colpo)
        {
            this.posIniziale = posIniziale;
            this.distanza = distanza;
            this.grade = grade;
            this.colpo = colpo;
        }
    }
}
