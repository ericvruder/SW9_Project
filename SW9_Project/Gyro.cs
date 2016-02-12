using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SW9_Project
{
    class Gyro
    {
        int timestamp;
        double runningCountZ = 0;
        double runningCountX = 0;
        int scale = 10;
        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        int screenHeight = Screen.PrimaryScreen.Bounds.Height;
        Cursor cursor = new Cursor(Cursor.Current.Handle); // For test

        public Gyro()
        {
            Cursor.Position = new Point(800, 450);
        }

        public void updateUI(string newX, string newY, string newZ)
        {
            
            double x = double.Parse(newX, CultureInfo.InvariantCulture);
            double y = double.Parse(newY, CultureInfo.InvariantCulture);
            double z = double.Parse(newZ, CultureInfo.InvariantCulture);

            /*
            double[,] rot = rvToRot(x, y, z);
            double[,] ya = new double[,] { { 0.0 }, { 1.0 }, { 0.0 } };
            double[,] xa = new double[,] { { 1.0 }, { 0.0 }, { 0.0 } };

            double[,] ty = MultiplyMatrix(rot, ya);
            double[,] tx = MultiplyMatrix(rot, xa);

            double cx = -ty[0,0] * int.Parse(screenWidth) + int.Parse(screenWidth) / 2.0;
            double cy = -ty[2,0] * int.Parse(screenHeight) + int.Parse(screenHeight) / 2.0;

            Cursor.Position = new Point((int)cx, (int)cy);*/

            runningCountX += x;
            runningCountZ += z;

            double cx = -z * ((screenWidth / 2.0)/0.16) + (screenWidth / 2.0);
            double cy = -x * ((screenHeight / 2.0)/0.11) + (screenHeight / 2.0);

            //Console.WriteLine("RC:" + x + "\t" + z);
            //Console.WriteLine("SC:" + cy + "\t" + cx);
            Cursor.Position = new Point((int)cx, (int)cy);
        }

        /// <summary>
        /// For testing
        /// </summary>
        public void moveCursor()
        {
            Cursor.Position = new Point(Cursor.Position.X - 250, Cursor.Position.Y - 250);
        }

        public double[,] MultiplyMatrix(double[,] a, double[,] b)
        {
            if (a.GetLength(1) == b.GetLength(0))
            {
                double[,] c = new double[a.GetLength(0), b.GetLength(1)];
                for (int i = 0; i < c.GetLength(0); i++)
                {
                    for (int j = 0; j < c.GetLength(1); j++)
                    {
                        c[i, j] = 0;
                        for (int k = 0; k < a.GetLength(1); k++)
                            c[i, j] = c[i, j] + a[i, k] * b[k, j];
                    }
                }
                return c;
            }
            else
            {
                return null;
            }
        }

        public double[,] rvToRot(double x, double y, double z)
        {
            double xx = Math.Pow(x, 2);
            double yy = Math.Pow(y, 2);
            double zz = Math.Pow(z, 2);
            double w = 1.0 - xx - yy - zz;
            double xy = x * y;
            double zw = z * w;
            double xz = x * z;
            double yw = y * w;
            double yz = y * z;
            double xw = x * w;

            double[,] rot = new double[,] { 
                { 1.0-2*yy-2*zz, 2*xy-2*zw, 2*xz+2*yw }, 
                { 2*xy+2*zw, 1.0-2*xx-2*zz, 2*yz-2*xw }, 
                { 2*xz-2*yw, 2*yz+2*xw, 1-2*xx-2*yy } };

            return rot;
        }
    }
}
