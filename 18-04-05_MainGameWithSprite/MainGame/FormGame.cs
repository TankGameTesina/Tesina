using GraficaTesina;
using SpriteLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainGame
{
    public partial class FormGame : Form
    {
        GenerateTank genTank;
        List<Bitmap> tank;
        List<Bitmap> tankEnemy;
        bool settato = false;
        double grade = 0;
        int spostamento =5;
        int centroCarroX;
        int centroCarroY;
        bool newColpo = false;
        List<Bullet> bullets = new List<Bullet>();

        SpriteController MySpriteController;
        Sprite JellyMonster;
        Sprite enemy;

        Point puntoInizioSparo;
        public FormGame()
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
            this.MouseClick += mouseClick;
            tank = genTank.GetTank();
            settato = true;

            MainDrawingArea.BackgroundImageLayout = ImageLayout.Stretch;

            MySpriteController = new SpriteController(MainDrawingArea);

            JellyMonster = new Sprite(MySpriteController, tank[0]);

            JellyMonster.SetName("jelly");


            JellyMonster.AutomaticallyMoves = false;
            JellyMonster.CannotMoveOutsideBox = true;
            JellyMonster.PutBaseImageLocation(new Point(0, 0));
            JellyMonster.MovementSpeed = 30;

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

            enemy = new Sprite(MySpriteController, tankEnemy[0]);

            enemy.SetName("enemy");

            enemy.AutomaticallyMoves = false;
            enemy.CannotMoveOutsideBox = true;
            enemy.PutBaseImageLocation(new Point(100, 100));
            enemy.MovementSpeed = 30;
        }

        /*
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        */
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
                    //double centroX = Screen.PrimaryScreen.Bounds.Width / 2;
                    //double centroY = Screen.PrimaryScreen.Bounds.Height / 2;
                    //grade = AngleFrom3PointsInDegrees(Cursor.Position.X, Cursor.Position.Y, centroX, centroY, centroX, centroY);
                    Point Image = this.PointToClient(JellyMonster.BaseImageLocation);
                    centroCarroX = Image.X + JellyMonster.GetSize.Width/2;
                    centroCarroY = Image.Y + JellyMonster.GetSize.Height/ 2;
                    Point pos = this.PointToClient(Cursor.Position);
                    grade = AngleFrom3PointsInDegrees(pos.X, pos.Y, centroCarroX, centroCarroY, centroCarroX, centroCarroY);
                    listBox1.Items.Insert(0, "position MOUSE: X: " + pos.X + "Y: " + pos.Y);
                    listBox1.Items.Insert(0, "Grade: " + grade.ToString());
                    /*grade = grade / 10;
                    if (grade % 10 != 0)
                        grade -= grade % 10;*/
                    JellyMonster.ReplaceImage(tank[Convert.ToInt32(grade)]);
                }
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            double xdegree = DegreeToRadian(grade);
            double cos = -Math.Cos(DegreeToRadian(grade));
            double sin = Math.Sin(DegreeToRadian(grade));
            switch (keyData)
            {
                case Keys.W:
                    int xw = Convert.ToInt32(JellyMonster.BaseImageLocation.X + (spostamento * sin));
                    int yw = Convert.ToInt32(JellyMonster.BaseImageLocation.Y + (spostamento * cos));
                    listBox1.Items.Insert(0, "position W: X: "+ xw + "Y: " + yw);
                    JellyMonster.PutBaseImageLocation(new Point(xw, yw));
                    break;
                case Keys.S:
                    int xs = Convert.ToInt32(JellyMonster.BaseImageLocation.X - (spostamento * sin));
                    int ys = Convert.ToInt32(JellyMonster.BaseImageLocation.Y - (spostamento * cos));
                    listBox1.Items.Insert(0, "position S: X: " + xs + "Y: " + ys);
                    JellyMonster.PutBaseImageLocation(new Point(xs, ys));
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void mouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                newColpo = true;
            }
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private void tmrSparo_Tick(object sender, EventArgs e)
        {
            Graphics formGraphics;
            SolidBrush myBrush = new SolidBrush(Color.Red);
            
            if (newColpo)
            {
                puntoInizioSparo = new Point(centroCarroX, centroCarroY);
                bullets.Add(new Bullet(puntoInizioSparo,0,grade));
                newColpo = false;
            }
            formGraphics = this.CreateGraphics();
            formGraphics.Clear(Color.Black);
            Rectangle colpo;
            foreach(Bullet bullet in bullets)
            {
                double xdegree = DegreeToRadian(bullet.grade);
                double cos = -Math.Cos(DegreeToRadian(bullet.grade));
                double sin = Math.Sin(DegreeToRadian(bullet.grade));

                colpo = new Rectangle(Convert.ToInt32(bullet.posIniziale.X + (sin * bullet.distanza)), Convert.ToInt32(bullet.posIniziale.Y + (cos * bullet.distanza))
                    , 5, 5);
                formGraphics.FillRectangle(myBrush, colpo);

                bullet.distanza += 10;
            }

        }
    }
}
