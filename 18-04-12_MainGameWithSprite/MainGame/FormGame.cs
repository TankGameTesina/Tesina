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
        bool newColpo = false;
        List<Bullet> bullets = new List<Bullet>();
        Bitmap colpo;
        List<Sprite> colpi = new List<Sprite>();
        SpriteController MySpriteController;
        Sprite JellyMonster;
        Sprite enemy;
        DateTime LastShot = DateTime.Now;
        Point puntoInizioSparo;
        public FormGame()
        {
            InitializeComponent();

            MouseMove += OnMouseMove;
            HookMouseMove(this.Controls);
            tank = new List<Bitmap>();
            genTank = new GenerateTank();
            colpo = new Bitmap("bullet.png");
            Random rnd = new Random();
            int red = rnd.Next(255); // creates a number between 1 and 12
            int green = rnd.Next(255);  // creates a number between 1 and 6
            int blue = rnd.Next(255);    // creates a number between 0 and 51
            genTank.generate(Color.FromArgb(red, green, blue));
            this.MouseClick += mouseClick;
            tank = genTank.GetTank();

            settato = true;
            //MainDrawingArea.BackgroundImage = Properties.Resources.map;
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
                    MouseEventArgs me = (MouseEventArgs)e;
                    Point coordinates = me.Location;
                    Point picture = JellyMonster.GetSpritePictureboxCenter();

                    grade = AngleFrom3PointsInDegrees(coordinates.X, coordinates.Y, picture.X, picture.Y, picture.X, picture.Y);
                    label1.Text = grade.ToString();
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
            double cos = -Math.Cos(DegreeToRadian(grade));
            double sin = Math.Sin(DegreeToRadian(grade));
            Point Image = JellyMonster.GetSpriteBaseImageCenter();
            Image = new Point(Image.X - JellyMonster.GetSize.Width / 2, Image.Y - JellyMonster.GetSize.Height / 2);
            switch (keyData)
            {
                case Keys.W:
                    int xw = Convert.ToInt32(Image.X + (spostamento * sin));
                    int yw = Convert.ToInt32(Image.Y + (spostamento * cos));
                    JellyMonster.PutBaseImageLocation(new Point(xw, yw));
                    break;
                case Keys.S:
                    int xs = Convert.ToInt32(Image.X - (spostamento * sin));
                    int ys = Convert.ToInt32(Image.Y - (spostamento * cos));
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
            if (newColpo)
            {
                //Check if we have had enough time since we last shot.  If so, we can shoot again
                TimeSpan Duration = DateTime.Now - LastShot;
                if (Duration.TotalMilliseconds > 300)
                {
                    //We make a new shot sprite.
                    Sprite newsprite = new Sprite(MySpriteController, colpo);
                    //Checking if newsprite==null is just a safeguard.  It is only null if we use a string that does not exist.
                    if (newsprite != null)
                    {
                        //We figure out where to put the shot
                        Point where = JellyMonster.GetSpritePictureboxCenter();
                        where = new Point(where.X, where.Y);
                        newsprite.PutPictureBoxLocation(where);
                        //We tell the sprite to automatically move
                        newsprite.AutomaticallyMoves = true;
                        //We give it a direction, up
                        double xdegree = DegreeToRadian(grade - 270);
                        newsprite.SetSpriteDirectionRadians(xdegree);
                        //we give it a speed for how fast it moves.
                        newsprite.MovementSpeed = 8;
                    }
                    LastShot = DateTime.Now;
                }
                newColpo = false;
            }

            /*Sprite colpoo;


            if (newColpo)
            {
                colpoo = new Sprite(MySpriteController, colpo,);
                puntoInizioSparo = JellyMonster.GetSpritePictureboxCenter();
                bullets.Add(new Bullet(puntoInizioSparo,0,grade, colpoo));
                newColpo = false;
            }

            foreach(Bullet bullet in bullets)
            {
                double xdegree = DegreeToRadian(bullet.grade);
                double cos = -Math.Cos(DegreeToRadian(bullet.grade));
                double sin = Math.Sin(DegreeToRadian(bullet.grade));

                // Sprite colpoo = new Sprite()
                bullet.colpo.PutBaseImageLocation(Convert.ToInt32(bullet.posIniziale.X + (sin * bullet.distanza)), Convert.ToInt32(bullet.posIniziale.Y + (cos * bullet.distanza)));

            }*/

        }

        private void MainDrawingArea_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                newColpo = true;
            }
        }
    }
}
