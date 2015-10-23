using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Timers;
using System.Threading;

namespace SW9_Project {
    class KinectManager {

        KinectSensor kinectSensor;
        string connectedStatus = "";
        IDrawingBoard board;

        double xScale, yScale;

        public KinectManager(IDrawingBoard board) {

            calibrationValues = new List<Tuple<double, double>>();
            this.board = board;

            DiscoverKinectSensor();
            StartKinect();
            System.Threading.Thread.Sleep(5000);

            Thread s = new Thread(Calibrate);
            s.SetApartmentState(ApartmentState.STA);
            s.Start();
        }
        

        List<Tuple<double,double>> calibrationValues;

        bool calibrating = true, gathering = false;

        private enum Corner { TopLeft, TopRight, BottomLeft, BottomRight }

        private List<Tuple<double, double>> GetCornerMeasurments(Corner corner) {

            
            //board.DrawNotice("Please point at the " + corner + " corner", 5);
            MessageWindow t = new MessageWindow("Please point at the " + corner + " corner");
            t.Show();
            System.Threading.Thread.Sleep(2000);
            gathering = true;
            System.Threading.Thread.Sleep(4000);
            gathering = false;
            t.Close();

            List<Tuple<double, double>> result = new List<Tuple<double, double>>();
            double x = 0, y = 0;
            lock (calibrationValues) {
                foreach(var cv in calibrationValues) {
                    result.Add(new Tuple<double, double>(cv.Item1, cv.Item2));
                    x += cv.Item1; y += cv.Item2;
                }
                x = x / calibrationValues.Count;
                y = y / calibrationValues.Count;
                calibrationValues.Clear();
            }

            
            //List<Tuple<double, double>> results = new List<Tuple<double, double>>(x,y);

            return result;
        }

        private void Calibrate() {

            List<List<Tuple<double, double>>> list = new List<List<Tuple<double, double>>>();

            list.Add(GetCornerMeasurments(Corner.TopLeft));
            list.Add(GetCornerMeasurments(Corner.TopRight));
            list.Add(GetCornerMeasurments(Corner.BottomRight));
            list.Add(GetCornerMeasurments(Corner.BottomLeft));
            
          

            CanvasWindow h = new CanvasWindow(true);
            h.Show();

            IDrawingBoard t = h;
            
            foreach (var j in list) {
                foreach (var g in j) {
                    t.AddDot(g.Item1, g.Item2);
                }
            }

            //System.Threading.Thread.Sleep(1000000);

            /*
            List<Tuple<double, double>> tl = GetCornerMeasurments(Corner.TopLeft);
            List<Tuple<double, double>> tr = GetCornerMeasurments(Corner.TopRight);
            List<Tuple<double, double>> br = GetCornerMeasurments(Corner.BottomRight);
            List<Tuple<double, double>> bl = GetCornerMeasurments(Corner.BottomLeft);*/


            /*
            double x = 0, y = 0;
            foreach (var value in calibrationValues) {
                x += value.Item1;
                y += value.Item2;
            }
            x = (x / calibrationValues.Count);
            y = (y / calibrationValues.Count);

            double w = board.Width() / 2, h = board.Height() / 2;

            xScale = w / x; yScale = h / y;

            xScale = 3000;
            yScale = 3000;
             

            calibrating = false;
            lock (calibrationValues) {
                calibrationValues.Clear();
            }
            */

        }

        private bool StartKinect() {

            kinectSensor.ColorStream.Enable();
            kinectSensor.SkeletonStream.Enable(new TransformSmoothParameters() {
                Smoothing = 0.5f,
                Correction = 0.5f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            });

            kinectSensor.AllFramesReady += KinectSensor_AllFramesReady;
            try {
                kinectSensor.Start();
            }
            catch(Exception e) {
                Console.WriteLine("Could not start kinect! E: " + e.Message);
                return false;
            }
            return true;

        }

        private void KinectSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e) {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) {
                if (skeletonFrame != null) {
                    Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    Skeleton playerSkeleton = (from s in skeletonData where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                    if (playerSkeleton != null) {
                        Joint rightHand = playerSkeleton.Joints[JointType.HandRight];
                        if (calibrating && gathering) {
                            lock (calibrationValues) {
                                calibrationValues.Add(new Tuple<double, double>(rightHand.Position.X, rightHand.Position.Y));
                            }
                        } else {
                            board.PointAt(xScale * rightHand.Position.X, yScale * rightHand.Position.Y);
                        }
                    }
                }
            }
        }

        private void DiscoverKinectSensor() {
            foreach (KinectSensor sensor in KinectSensor.KinectSensors) {
                if (sensor.Status == KinectStatus.Connected) {
                    // Found one, set our sensor to this
                    kinectSensor = sensor;
                    break;
                }
            }

            if (this.kinectSensor == null) {
                connectedStatus = "Found none Kinect Sensors connected to USB";
                return;
            }
        }

        private void KinectSensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e) {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) {
                if (skeletonFrame != null) {
                    Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    Skeleton playerSkeleton = (from s in skeletonData where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                    if (playerSkeleton != null) {
                        Joint rightHand = playerSkeleton.Joints[JointType.HandRight];
                        Console.WriteLine(rightHand.Position.X + ", " + rightHand.Position.Y);
                        //handPosition = new Vector2((((0.5f * rightHand.Position.X) + 0.5f) * (640)), (((-0.5f * rightHand.Position.Y) + 0.5f) * (480)));
                    }
                }
            }
        }
    }


}
