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
        double yScale = 0.25f;
        double zScale = 0.25f;
        double x, y, z;
        int xScaleRaw = 200;
        int yScaleRaw = 50;
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
                x = Math.Round(double.Parse(nX, CultureInfo.InvariantCulture), 10) / xScaleRaw;
                y = Math.Round(double.Parse(nY, CultureInfo.InvariantCulture), 10) / yScaleRaw;
                z = Math.Round(double.Parse(nZ, CultureInfo.InvariantCulture), 10) / zScaleRaw ;
                long currentTime = long.Parse(nTime, CultureInfo.InvariantCulture);
                Console.WriteLine("x:" + x + "\t y:" + y + "\t z:" + z);

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

            double cx = (((screenWidth / 0.25) / 2.0) * x) + (screenWidth / 2.0);
            double cy = (((screenHeight / 0.25) / 2.0) * y) + (screenHeight / 2.0);

            //Console.WriteLine("RC:" + runningCountX + "\t" + runningCountZ);
            //Console.WriteLine("SC:" + cx + "\t" + cy);
            Cursor.Position = new Point((int)cx, (int)cy);
            //CanvasWindow.GyroPositionX = x;
            //CanvasWindow.GyroPositionY = y;
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
