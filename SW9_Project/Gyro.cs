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
        double runningCountZ = 0;
        double runningCountX = 0;
        double xScale = 72;
        double zScale = 72;
        long previousTime = 0;
        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        int screenHeight = Screen.PrimaryScreen.Bounds.Height;
        Cursor cursor = new Cursor(Cursor.Current.Handle); // For test

        public Gyro()
        {
            Cursor.Position = new Point(800, 450);
        }

        public void updateUI(string nTime, string nX, string nY, string nZ)
        {
            try {
                double x = Math.Round(double.Parse(nX, CultureInfo.InvariantCulture), 1);
                //double y = double.Parse(nY, CultureInfo.InvariantCulture);
                double z = Math.Round(double.Parse(nZ, CultureInfo.InvariantCulture), 1); // Throws exeption here sometimes
                long currentTime = long.Parse(nTime, CultureInfo.InvariantCulture);

                if (currentTime < previousTime)
                    return;

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
                previousTime = currentTime;
            }
            catch (FormatException e)
            {
            }

            RunningCountLimit(ref runningCountX, ref runningCountZ);

            double cx = -runningCountZ * ((screenWidth / 2.0) / zScale) + (screenWidth / 2.0);
            double cy = -runningCountX * ((screenHeight / 2.0) / xScale) + (screenHeight / 2.0);

            // TODO move the pointer and not the system cursor

            Console.WriteLine("RC:" + runningCountX + "\t" + runningCountZ);
            Console.WriteLine("SC:" + cy + "\t" + cx);
            Cursor.Position = new Point((int)cx, (int)cy);
        }

        private void RunningCountLimit(ref double x, ref double z)
        {
            if (x > xScale)
                x = xScale;
            else if (x < -xScale)
                x = -xScale;
            if (z > zScale)
                z = zScale;
            else if (z < -zScale)
                z = -zScale;
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
