using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainGame
{
    class Line
    {
        public Point p1, p2;

        public Line(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        private double GetDistance()
        {
            return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
        }

        public Point[] getPoints(int quantity)
        {
            int distanceSquared = Convert.ToInt32(GetDistance());
            var points = new Point[quantity + distanceSquared];
            int ydiff = p2.Y - p1.Y, xdiff = p2.X - p1.X;
            double slope = (double)(p2.Y - p1.Y) / (p2.X - p1.X);
            double x, y;

            --quantity;

            for (double i = 0; i < quantity + distanceSquared; i++)
            {
                y = slope == 0 ? 0 : ydiff * (i / quantity);
                x = slope == 0 ? xdiff * (i / quantity) : y / slope;
                points[(int)i] = new Point((int)Math.Round(x) + p1.X, (int)Math.Round(y) + p1.Y);
            }

            points[quantity] = p2;
            return points;
        }

        public List<Point> GetPoints(Point p1, Point p2)
        {
            List<Point> points = new List<Point>();

            // no slope (vertical line)
            if (p1.X == p2.X)
            {
                for (int y = p1.Y; y <= p2.Y; y++)
                {
                    Point p = new Point(p1.X, y);
                    points.Add(p);
                }
            }
            else
            {
                // swap p1 and p2 if p2.X < p1.X
                if (p2.X < p1.X)
                {
                    Point temp = p1;
                    p1 = p2;
                    p2 = temp;
                }

                double deltaX = p2.X - p1.X;
                double deltaY = p2.Y - p1.Y;
                double error = -1.0f;
                double deltaErr = Math.Abs(deltaY / deltaX);

                int y = p1.Y;
                for (int x = p1.X; x <= p2.X; x++)
                {
                    Point p = new Point(x, y);
                    points.Add(p);

                    error += deltaErr;

                    while (error >= 0.0f)
                    {
                        y++;
                        points.Add(new Point(x, y));
                        error -= 1.0f;
                    }
                }

                if (points.Last() != p2)
                {
                    int index = points.IndexOf(p2);
                    points.RemoveRange(index + 1, points.Count - index - 1);
                }
            }

            return points;
        }

    }
}
