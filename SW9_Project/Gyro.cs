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
        int scale = 333;
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
                double x = Math.Round(double.Parse(nX, CultureInfo.InvariantCulture), 10) / scale;
                //double y = double.Parse(nY, CultureInfo.InvariantCulture);
                double z = Math.Round(double.Parse(nZ, CultureInfo.InvariantCulture), 10) / scale ; // Throws exeption here sometimes
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

            RunningCountLimit(ref runningCountX, ref runningCountZ);

            double cx = -runningCountZ * ((screenWidth / 2.0) / zScale) + (screenWidth / 2.0);
            double cy = -runningCountX * ((screenHeight / 2.0) / xScale) + (screenHeight / 2.0);

            // TODO move the pointer and not the system cursor

            //Console.WriteLine("RC:" + runningCountX + "\t" + runningCountZ);
            //Console.WriteLine("SC:" + cy + "\t" + cx);
            Cursor.Position = new Point((int)cx, (int)cy);
            CanvasWindow.GyroPositionX = -runningCountZ;
            CanvasWindow.GyroPositionY = -runningCountX;
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
