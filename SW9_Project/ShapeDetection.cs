using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System.Drawing;
using System.Diagnostics;

namespace SW9_Project {
    class ShapeDetection {

        private void DetectCirclesOne() { 
            
            /*

            Mat imgOriginal = frame.Mat;

            if (imgOriginal == null) {
                MessageBox.Show("unable to read from webcam" + Environment.NewLine + Environment.NewLine + "exiting program");
                Environment.Exit(0);
                return;
            }

            Mat imgHSV = new Mat(imgOriginal.Size, DepthType.Cv8U, 3);

            Mat imgThreshLow = new Mat(imgOriginal.Size, DepthType.Cv8U, 1);
            Mat imgThreshHigh = new Mat(imgOriginal.Size, DepthType.Cv8U, 1);

            Mat imgThresh = new Mat(imgOriginal.Size, DepthType.Cv8U, 1);

            CvInvoke.CvtColor(imgOriginal, imgHSV, ColorConversion.Bgr2Hsv);

            CvInvoke.InRange(imgHSV, new ScalarArray(new MCvScalar(0, 155, 155)), new ScalarArray(new MCvScalar(18, 255, 255)), imgThreshLow);
            CvInvoke.InRange(imgHSV, new ScalarArray(new MCvScalar(165, 155, 155)), new ScalarArray(new MCvScalar(179, 255, 255)), imgThreshHigh);

            CvInvoke.Add(imgThreshLow, imgThreshHigh, imgThresh);

            CvInvoke.GaussianBlur(imgThresh, imgThresh, new Size(3, 3), 0);

            Mat structuringElement = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));

            CvInvoke.Dilate(imgThresh, imgThresh, structuringElement, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(0, 0, 0));
            CvInvoke.Erode(imgThresh, imgThresh, structuringElement, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(0, 0, 0));

            CircleF[] circles = CvInvoke.HoughCircles(imgThresh, HoughType.Gradient, 2.0, imgThresh.Rows / 4, 100, 50, 10, 400);

            foreach (CircleF circle in circles) {
                if (txtXYRadius.Text != "") {                         // if we are not on the first line in the text box
                    txtXYRadius.AppendText(Environment.NewLine);         // then insert a new line char
                }

                txtXYRadius.AppendText("ball position x = " + circle.Center.X.ToString().PadLeft(4) + ", y = " + circle.Center.Y.ToString().PadLeft(4) + ", radius = " + circle.Radius.ToString("###.000").PadLeft(7));
                txtXYRadius.ScrollToCaret();             // scroll down in text box so most recent line added (at the bottom) will be shown

                CvInvoke.Circle(imgOriginal, new Point((int)circle.Center.X, (int)circle.Center.Y), (int)circle.Radius, new MCvScalar(0, 0, 255), 2);
                CvInvoke.Circle(imgOriginal, new Point((int)circle.Center.X, (int)circle.Center.Y), 3, new MCvScalar(0, 255, 0), -1);
            }
            ibOriginal.Image = imgOriginal;
            ibThresh.Image = imgThresh;
            */

        }

        public IImage DetectShapes(Image<Bgr,byte> img) {
            StringBuilder msgBuilder = new StringBuilder("Performance: ");

            //Convert the image to grayscale and filter out the noise
            UMat uimage = new UMat();
            CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

            //use image pyr to remove noise
            UMat pyrDown = new UMat();
            CvInvoke.PyrDown(uimage, pyrDown);
            CvInvoke.PyrUp(pyrDown, uimage);

            //Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

            #region circle detection
            Stopwatch watch = Stopwatch.StartNew();
            double cannyThreshold = 180.0;
            double circleAccumulatorThreshold = 120;
            CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);

            watch.Stop();
            msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));
            #endregion

            #region Canny and edge detection
            watch.Reset(); watch.Start();
            double cannyThresholdLinking = 120.0;
            UMat cannyEdges = new UMat();
            CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);

            LineSegment2D[] lines = CvInvoke.HoughLinesP(
               cannyEdges,
               1, //Distance resolution in pixel-related units
               Math.PI / 45.0, //Angle resolution measured in radians.
               20, //threshold
               30, //min Line width
               10); //gap between lines

            watch.Stop();
            msgBuilder.Append(String.Format("Canny & Hough lines - {0} ms; ", watch.ElapsedMilliseconds));
            #endregion

            #region Find triangles and rectangles
            watch.Reset(); watch.Start();
            List<Triangle2DF> triangleList = new List<Triangle2DF>();
            List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle

            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint()) {
                CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                int count = contours.Size;
                for (int i = 0; i < count; i++) {
                    using (VectorOfPoint contour = contours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint()) {
                        CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                        if (CvInvoke.ContourArea(approxContour, false) > 250) //only consider contours with area greater than 250
                        {
                            if (approxContour.Size == 3) //The contour has 3 vertices, it is a triangle
                            {
                                Point[] pts = approxContour.ToArray();
                                triangleList.Add(new Triangle2DF(
                                   pts[0],
                                   pts[1],
                                   pts[2]
                                   ));
                            } else if (approxContour.Size == 4) //The contour has 4 vertices.
                            {
                                #region determine if all the angles in the contour are within [80, 100] degree
                                bool isRectangle = true;
                                Point[] pts = approxContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                for (int j = 0; j < edges.Length; j++) {
                                    double angle = Math.Abs(
                                       edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                    if (angle < 80 || angle > 100) {
                                        isRectangle = false;
                                        break;
                                    }
                                }
                                #endregion

                                if (isRectangle) boxList.Add(CvInvoke.MinAreaRect(approxContour));
                            }
                        }
                    }
                }
            }

            watch.Stop();
            msgBuilder.Append(String.Format("Triangles & Rectangles - {0} ms; ", watch.ElapsedMilliseconds));
            #endregion

            Image<Bgr, byte> drawn = img.CopyBlank();

            foreach(Triangle2DF triangle in triangleList) {
                drawn.Draw(triangle, new Bgr(Color.DarkBlue), 2);
            }
            foreach(RotatedRect box in boxList) {
                drawn.Draw(box, new Bgr(Color.DarkBlue), 2);
            }

            foreach(CircleF circle in circles) {
                drawn.Draw(circle, new Bgr(Color.DarkRed), 2);
            }

            return drawn;

            /*
            originalImageBox.Image = img;
            this.Text = msgBuilder.ToString();

            #region draw triangles and rectangles
            Image<Bgr, Byte> triangleRectangleImage = img.CopyBlank();
            foreach (Triangle2DF triangle in triangleList)
                triangleRectangleImage.Draw(triangle, new Bgr(Color.DarkBlue), 2);
            foreach (RotatedRect box in boxList)
                triangleRectangleImage.Draw(box, new Bgr(Color.DarkOrange), 2);
            triangleRectangleImageBox.Image = triangleRectangleImage;
            #endregion

            #region draw circles
            Image<Bgr, Byte> circleImage = img.CopyBlank();
            foreach (CircleF circle in circles)
                circleImage.Draw(circle, new Bgr(Color.Brown), 2);
            circleImageBox.Image = circleImage;
            #endregion

            #region draw lines
            Image<Bgr, Byte> lineImage = img.CopyBlank();
            foreach (LineSegment2D line in lines)
                lineImage.Draw(line, new Bgr(Color.Green), 2);
            lineImageBox.Image = lineImage;
            #endregion
    */
        }
    }
}
