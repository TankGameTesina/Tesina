using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraficaTesina
{
    public partial class Form1 : Form
    {
        Color colore;
        string fileName;
        string pathName;

        private static void RotateAndSaveImage(Bitmap sourceImage, string output, int angle)
        {
            //Open the source image and create the bitmap for the rotatated image
            using (Bitmap rotateImage = new Bitmap(sourceImage.Width, sourceImage.Height))
            {
                //Set the resolution for the rotation image
                rotateImage.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                //Create a graphics object
                using (Graphics gdi = Graphics.FromImage(rotateImage))
                {
                    //Rotate the image
                    gdi.TranslateTransform((float)sourceImage.Width / 2, (float)sourceImage.Height / 2);
                    gdi.RotateTransform(angle);
                    gdi.TranslateTransform(-(float)sourceImage.Width / 2, -(float)sourceImage.Height / 2);
                    gdi.DrawImage(sourceImage, new System.Drawing.Point(0, 0));
                }

                //Save to a file
                rotateImage.Save(output);
            }
        }

        private static Bitmap ChangeToColor(Bitmap bmp, Color c)
        {
            Bitmap bmp2 = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics g = Graphics.FromImage(bmp2))
            {
                float tr = c.R / 255f;
                float tg = c.G / 255f;
                float tb = c.B / 255f;

                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                  {
                 new float[] {0, 0, 0, 0, 0},
                 new float[] {0, 0, 0, 0, 0},
                 new float[] {0, 0, 0, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {tr, tg, tb, 0, 1}  // kudos to OP!
                  });

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);

                g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height),
                    0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
            }
            return bmp2;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void btnConferma_Click(object sender, EventArgs e)
        {
            fileName = Path.GetFileName(tbPath.Text);
            pathName = Path.GetDirectoryName(tbPath.Text);

            Bitmap colorata = ChangeToColor(new Bitmap(tbPath.Text), colore);

            Directory.CreateDirectory(pathName + @"\tank_" + colore.Name);

            for (int i = 0; i <= 360; i++)
                RotateAndSaveImage(colorata, pathName + @"\tank_"+ colore.Name + @"\tank_" + i + ".png",i);
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

        private void btnColore_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                picColore.BackColor = colorDialog1.Color;
                colore = Color.FromArgb(colorDialog1.Color.ToArgb());
            }
        }
    }
}
