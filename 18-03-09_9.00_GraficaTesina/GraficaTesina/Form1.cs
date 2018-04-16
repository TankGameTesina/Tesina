using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GraficaTesina
{
    public partial class Form1 : Form
    {
        GenerateTank genTank;
        List<Bitmap> tank;
        List<List<Bitmap>> tanks;
        List<PictureBox> pics;
        bool settato = false;
        int numTanks = 25;
        public Form1()
        {
            InitializeComponent();

            MouseMove += OnMouseMove;
            HookMouseMove(this.Controls);
            progressBar1.Maximum = numTanks;
            progressBar1.Step = 1;

            tank = new List<Bitmap>();
            tanks = new List<List<Bitmap>>();
            pics = new List<PictureBox>();
            for(int i = 0; i < numTanks; i++)
            {
                PictureBox pictureBox = new PictureBox();

                flowLayoutPanel1.Controls.Add(pictureBox);

                pics.Add(pictureBox);
            }

            //thread = new Thread(new ThreadStart(this.AggiornaContatore));

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
                if(settato)
                {
                    double centroX = Screen.PrimaryScreen.Bounds.Width / 2;
                    double centroY = Screen.PrimaryScreen.Bounds.Height / 2;
                    double grade = AngleFrom3PointsInDegrees(Cursor.Position.X, Cursor.Position.Y, centroX, centroY, centroX, centroY);
                    label1.Text = grade.ToString();
                    for(int i = 0; i < numTanks; i++)
                    {
                        pics[i].Image = tanks[i][Convert.ToInt32(grade)];
                    }
                }
            }
        }
        private void btnConferma_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < numTanks; i++)
            {
                if (tbPath.Text != "")
                    // if (colorDialog1.ShowDialog() == DialogResult.OK)
                     {
                        Random rnd = new Random();
                        int red = rnd.Next(255); // creates a number between 1 and 12
                        int green = rnd.Next(255);  // creates a number between 1 and 6
                        int blue = rnd.Next(255);    // creates a number between 0 and 51
                    genTank = new GenerateTank(Color.FromArgb(red,green,blue), tbPath.Text, true);

                        tank = genTank.GetTank();
                    }
                tanks.Add(tank);
                progressBar1.PerformStep();
            }
            settato = true;
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.InitialDirectory = @"\";
                openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        tbPath.Text = openFileDialog1.FileName;
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
        private bool MouseInControl(Control ctrl)
        {
            return ctrl.Bounds.Contains(ctrl.PointToClient(MousePosition));
        }

        /*private void AggiornaContatore()
        {
            while(true)
            {
                if (this.label1.InvokeRequired)
                {
                    this.label1.BeginInvoke((MethodInvoker)delegate () {
                        if(MouseInControl(this))
                        {
                            double grade = AngleFrom3PointsInDegrees(Cursor.Position.X, Cursor.Position.Y, this.Width + pictureBox10.Width, this.Height + pictureBox10.Height, this.Width + pictureBox10.Width, this.Height + pictureBox10.Height);
                            label1.Text = grade.ToString();
                            pictureBox10.Image = tanks[Convert.ToInt32(grade)];
                        }
                    });
                }
                else
                {
                    /*double centroX = Screen.PrimaryScreen.Bounds.Width / 2;
                    double centroY = Screen.PrimaryScreen.Bounds.Height / 2;
                    double grade = AngleFrom3PointsInDegrees(Cursor.Position.X, Cursor.Position.Y, centroX, centroY, centroX, centroY);
                    
                    if (MouseInControl(this))
                    {
                        double grade = AngleFrom3PointsInDegrees(Cursor.Position.X, Cursor.Position.Y, this.Width + pictureBox10.Width, this.Height + pictureBox10.Height, this.Width + pictureBox10.Width, this.Height + pictureBox10.Height);
                        label1.Text = grade.ToString();
                        pictureBox10.Image = tanks[Convert.ToInt32(grade)];
                    }
                }
                Thread.Sleep(10);
            }
        }*/

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
                switch (keyData)
                {
                    case Keys.Left:
                    for (int i = 0; i < numTanks; i++)
                    {
                        pics[i].Left -= 10;
                    }
                        break;
                    case Keys.Right:
                    for (int i = 0; i < numTanks; i++)
                    {
                        pics[i].Left += 10;
                    }
                    break;
                }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
