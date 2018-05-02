using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestTesinaTrasparenza
{
    public partial class Form1 : Form
    {
        GenerateTank genTank;
        List<Bitmap> tank;
        List<Bitmap> tankEnemy;
        double grade = 0;
        double oldGrade = -1;
        int spostamento = 5;
        int centroCarroX;
        int centroCarroY;
        bool settato = false;

        bool newColpo = false;
        List<Bullet> bullets = new List<Bullet>();
        Point puntoInizioSparo;


        public Form1()
        {
            InitializeComponent();
            MouseMove += OnMouseMove;
            HookMouseMove(this.Controls);
            tank = new List<Bitmap>();
            genTank = new GenerateTank();

            Random rnd = new Random();
            int red = rnd.Next(255); // creates a number between 1 and 12
            int green = rnd.Next(255);  // creates a number between 1 and 6
            int blue = rnd.Next(255);    // creates a number between 0 and 51
            genTank.generate(Color.FromArgb(red, green, blue));
            tank = genTank.GetTank();
            settato = true;

            generateEnemy();


            }

        private void generateEnemy()
        {
            Random rnd = new Random();
            int red = rnd.Next(255); // creates a number between 1 and 12
            int green = rnd.Next(255);  // creates a number between 1 and 6
            int blue = rnd.Next(255);    // creates a number between 0 and 51
            genTank.generate(Color.FromArgb(red, green, blue));
            tankEnemy = genTank.GetTank();

            picEnemy.Image = tankEnemy[270];
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private Color getColorPunta(Point center)
        {
            int r = 25;
            double x = center.X + r * Math.Cos(DegreeToRadian(grade));
            double y = center.Y + r * Math.Sin(DegreeToRadian(grade));
            label2.Text = "X: " + x + "|Y: " + y;
            return GetColorAt(new Point(Convert.ToInt32(x), Convert.ToInt32(y)));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            double xdegree = DegreeToRadian(grade);
            double cos = -Math.Cos(DegreeToRadian(grade));
            double sin = Math.Sin(DegreeToRadian(grade));
            switch (keyData)
            {
                case Keys.W:
                    int xw = Convert.ToInt32(picTank.Location.X + (spostamento * sin));
                    int yw = Convert.ToInt32(picTank.Location.Y + (spostamento * cos));
                    picTank.Location = new Point(xw, yw);
                    label3.Text = getColorPunta(new Point(xw + 50, yw + 50)).ToString();
                    break;
                case Keys.S:
                    int xs = Convert.ToInt32(picTank.Location.X - (spostamento * sin));
                    int ys = Convert.ToInt32(picTank.Location.Y - (spostamento * cos));
                    picTank.Location = new Point(xs, ys);
                    label3.Text = getColorPunta(new Point(xs + 50, xs + 50)).ToString();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void HookMouseMove(Control.ControlCollection ctls)
        {
            foreach (Control ctl in ctls)
            {
                ctl.MouseMove += OnMouseMove;
                HookMouseMove(ctl.Controls);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (ClientRectangle.Contains(e.Location))
            {
                if (settato)
                {
                    centroCarroX = picTank.Location.X + picTank.Width / 2;
                    centroCarroY = picTank.Location.Y + picTank.Height / 2;
                    grade = AngleFrom3PointsInDegrees(Cursor.Position.X, Cursor.Position.Y, centroCarroX, centroCarroY, centroCarroX, centroCarroY);
                    label1.Text = grade.ToString();
                }
            }
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
        Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);

        public Color GetColorAt(Point location)
        {
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }

            return screenPixel.GetPixel(0, 0);
        }


        private void tmrMovimento_Tick(object sender, EventArgs e)
        {
            if (settato)
            {
                if(grade != oldGrade)
                {
                    picTank.SuspendLayout();
                    picTank.Image = tank[Convert.ToInt32(grade)];
                    oldGrade = grade;
                }
                //label3.Text = "X: " + picTank.Location.X + "| Y: " + picTank.Location.Y;
                //label2.Text = picTank.colore.ToString();
            }
        }


        private double AngleFrom3PointsInDegrees(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            double a = x2 - x1;
            double b = y2 - y1;
            double c = x3 - x2;
            double d = y3 - y2;

            double atanA = Math.Atan2(a, b);
            double atanB = Math.Atan2(c, d);

            double risultato = (atanA - atanB) * (-180 / Math.PI);

            if (risultato < 0)
                risultato = 360 + risultato;

            return risultato;

        }

        private void mouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                newColpo = true;
            }
        }

        /*private void tmrSparo_Tick(object sender, EventArgs e)
        {
            Graphics formGraphics;
            SolidBrush myBrush = new SolidBrush(Color.Red);

            if (newColpo)
            {
                puntoInizioSparo = new Point(centroCarroX, centroCarroY);
                bullets.Add(new Bullet(puntoInizioSparo, 0, grade));
                newColpo = false;
            }
            formGraphics = this.CreateGraphics();
            Rectangle colpo;
            foreach (Bullet bullet in bullets)
            {
                double xdegree = DegreeToRadian(bullet.grade);
                double cos = -Math.Cos(DegreeToRadian(bullet.grade));
                double sin = Math.Sin(DegreeToRadian(bullet.grade));

                colpo = new Rectangle(Convert.ToInt32(bullet.posIniziale.X + (sin * bullet.distanza)), Convert.ToInt32(bullet.posIniziale.Y + (cos * bullet.distanza))
                    , 5, 5);
                formGraphics.FillRectangle(myBrush, colpo);

                bullet.distanza += 1;
            }

        }*/
    }
}
