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
using ImageWorker;
using Timer = System.Windows.Forms.Timer;

namespace Lab2
{
    class ImageProcess : ImageHandler
    {
        public string HandlerName { get; }

        public Bitmap Source { set { source = value; } }

        public Bitmap Result { get { return result; } }

        private Bitmap source;
        private Bitmap result;

        private int Contrast, Brightness, R, G, B;

        public ImageProcess(int num)
        {
            HandlerName = num.ToString();
        }

        public void init(SortedList<string, object> parameters)
        {
            Contrast = (int)parameters["Contrast"];
            Brightness = (int)parameters["Brightness"];
            R = (int)parameters["R"];
            G = (int)parameters["G"];
            B = (int)parameters["B"];
        }

        public void startHandle(ProgressDelegate progress)
        {
            UInt32[,] pixel = new UInt32[source.Height, source.Width];
            
            for (int y = 0; y < source.Height; y++)
                for (int x = 0; x < source.Width; x++)
                    pixel[y, x] = (UInt32)source.GetPixel(x, y).ToArgb();

            double progr = 0;
            

            double a = 1;

            if (Brightness != 0)
                a++;
            if (Contrast != 0)
                a++;
            if (R != 0)
                a++;
            if (G != 0)
                a++;
            if (B != 0)
                a++;

            double incr = (1d / a) / (double)source.Height;


            if (Brightness != 0)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    for (int x = 0; x < source.Width; x++)
                    {
                        pixel[y, x] = BrightnessContrast.Brightness(pixel[y, x], Brightness, 10);
                    }
                    progr += incr;
                    progress.Invoke(progr);
                }
            }

            if (Contrast != 0)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    for (int x = 0; x < source.Width; x++)
                    {
                        pixel[y, x] = BrightnessContrast.Contrast(pixel[y, x], Contrast, 10);
                    }
                    progr += incr;
                    progress.Invoke(progr);
                }
            }

            if (R != 0)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    for (int x = 0; x < source.Width; x++)
                    {
                        pixel[y, x] = ColorBalance.ColorBalance_R(pixel[y, x], R, 10);
                    }
                    progr += incr;
                    progress.Invoke(progr);
                }
            }

            if (G != 0)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    for (int x = 0; x < source.Width; x++)
                    {
                        pixel[y, x] = ColorBalance.ColorBalance_G(pixel[y, x], G, 10);
                    }
                    progr += incr;
                    progress.Invoke(progr);
                }
            }

            if (B != 0)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    for (int x = 0; x < source.Width; x++)
                    {
                        pixel[y, x] = ColorBalance.ColorBalance_B(pixel[y, x], B, 10);
                    }
                    progr += incr;
                    progress.Invoke(progr);
                }
            }

            result = new Bitmap(source.Width, source.Height);

            for (int x = 0; x < source.Height; x++)
            {
                for (int y = 0; y < source.Width; y++)
                {
                    result.SetPixel(y, x, Color.FromArgb((int)pixel[x, y]));
                }
                progr += incr;
                progress.Invoke(progr);
            }
        }
    }
}
