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
    class GyroParser
    {
        double runningCountZ = 0;
        double runningCountX = 0;
        double xScale = 0.26f;
        double zScale = 0.25f;
        int xScaleRaw = 200;
        int zScaleRaw = 260;
        long previousTime = 0;
        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        int screenHeight = Screen.PrimaryScreen.Bounds.Height;
        Cursor cursor = new Cursor(Cursor.Current.Handle); // For test

        public GyroParser()
        {
            Cursor.Position = new Point(800, 450);
        }

        public void Update(string nTime, string nX, string nY, string nZ)
        {
            try {
                double x = Math.Round(double.Parse(nX, CultureInfo.InvariantCulture), 10) / xScaleRaw;
                //double y = double.Parse(nY, CultureInfo.InvariantCulture);
                double z = Math.Round(double.Parse(nZ, CultureInfo.InvariantCulture), 10) / zScaleRaw ; // Throws exeption here sometimes
                long currentTime = long.Parse(nTime, CultureInfo.InvariantCulture);

                if (currentTime < previousTime)
                    return;

                runningCountX += x;
                runningCountZ += z;
                previousTime = currentTime;
            }
            catch (FormatException e)
            {
            }

            //RunningCountLimit(ref runningCountX, ref runningCountZ);

            double cx = -runningCountZ * ((screenWidth / zScale) / 2.0) + (screenWidth / 2.0);
            double cy = -runningCountX * ((screenHeight / xScale) / 2.0) + (screenHeight / 2.0);

            //Console.WriteLine("RC:" + runningCountX + "\t" + runningCountZ);
            //Console.WriteLine("SC:" + cy + "\t" + cx);
            //Cursor.Position = new Point((int)cx, (int)cy);
            CanvasWindow.GyroPositionX = -runningCountZ;
            CanvasWindow.GyroPositionY = -runningCountX;
            //Console.WriteLine("X:" + -runningCountZ + " Y:" + -runningCountX);
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
    }
}
