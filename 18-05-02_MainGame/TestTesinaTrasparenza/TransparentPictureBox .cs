using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TestTesinaTrasparenza
{
    public class TransparentPictureBox : Control
    {
        private Timer refresher;
        private Image _image = null;
        public Color colore = new Color();
        public Point OldPos = new Point();
        public int OldGrade = 0;
        public Point pos = new Point();
        public int grade = 0;

        public TransparentPictureBox()
        {
            refresher = new Timer();
            refresher.Tick += new EventHandler(this.TimerOnTick);
            refresher.Interval = 50;
            refresher.Start();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        protected override void OnMove(EventArgs e)
        {
            base.RecreateHandle();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_image != null)
            {
                e.Graphics.DrawImage(_image, (Width / 2) - (_image.Width / 2), (Height / 2) - (_image.Height / 2));
            }
        }


        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        private void TimerOnTick(object source, EventArgs e)
        {
            base.RecreateHandle();
            refresher.Stop();
        }

        public Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                base.RecreateHandle();
            }
        }
    }
}
