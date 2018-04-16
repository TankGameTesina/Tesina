using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GraficaTesina
{
    class GenerateTank
    {
        private Color colore;
        private string fileName;
        private string pathName;
        private bool savePic;

        public bool generate(Color colore)
        {
            try
            {

                this.Colore = colore;
                string pathFile = downloadImage();
                this.FileName = Path.GetFileName(pathFile);
                this.PathName = Path.GetDirectoryName(pathFile);
                this.SavePic = savePic;
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public string downloadImage()
        {
            string startupPath = Environment.CurrentDirectory;
            string localFilename = startupPath + @"\tankScaricato.png";

            if(!File.Exists(localFilename))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("http://www.itis.pr.it/~smichelo1/file/Image/tank.png", localFilename);
                }
            }

            return localFilename;
        }

        public GenerateTank() {
            savePic = true;
            if (Directory.Exists(PathName + @"\tanks"))
                Directory.Delete(PathName + @"\tanks", true);
            else
                Directory.CreateDirectory(PathName + @"\tanks");
        }

        public Color Colore
        {
            get
            {
                return colore;
            }

            set
            {
                colore = value;
            }
        }

        public string FileName
        {
            get
            {
                return fileName;
            }

            set
            {
                fileName = value;
            }
        }

        public string PathName
        {
            get
            {
                return pathName;
            }

            set
            {
                pathName = value;
            }
        }

        public bool SavePic
        {
            get
            {
                return savePic;
            }

            set
            {
                savePic = value;
            }
        }

        private static bool RotateAndSaveImage(Bitmap sourceImage, string output, int angle, bool saveOnFile)
        {
            using (Bitmap rotateImage = new Bitmap(sourceImage.Width, sourceImage.Height))
            {
                rotateImage.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                using (Graphics gdi = Graphics.FromImage(rotateImage))
                {
                    gdi.TranslateTransform((float)sourceImage.Width / 2, (float)sourceImage.Height / 2);
                    gdi.RotateTransform(angle);
                    gdi.TranslateTransform(-(float)sourceImage.Width / 2, -(float)sourceImage.Height / 2);
                    gdi.DrawImage(sourceImage, new System.Drawing.Point(0, 0));
                }

                if(saveOnFile)
                    rotateImage.Save(output);

                return true;

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

        public List<Bitmap> GetTank()
        {
            if(FileName == null || PathName == null || Colore == null)
                return null;

            List<Bitmap> bitmaps = new List<Bitmap>();
            Bitmap colorata = ChangeToColor(new Bitmap(PathName+@"\"+FileName), Colore);

            Directory.CreateDirectory(PathName + @"\tanks\tank_" + Colore.Name);

            for (int i = 0; i <= 360; i ++)
            {
                if (RotateAndSaveImage(colorata, PathName + @"\tanks\tank_" + Colore.Name + @"\tank_" + i + ".png", i, SavePic))
                    bitmaps.Add(new Bitmap(PathName + @"\tanks\tank_" + Colore.Name + @"\tank_" + i + ".png"));
            }


            return bitmaps;
        }


    }
}
