using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using System.ComponentModel;
using System.Timers;
using System.Windows;

using DataSetGenerator;
using Point = System.Windows.Point;
using System.Windows.Media.Imaging;

namespace SW9_Project {
    class KinectManager {

        KinectSensor kinectSensor;
        IDrawingBoard board;
        Body[] bodies;

        private Timer handChangeTimer;
        private double handChangeTime = 2; //seconds to wait for hand change

        public KinectManager(IDrawingBoard board) {
            
            this.board = board;

            StartKinect();
            
        }

        private bool LeftHand = true;

        InfraredFrameReader irReader;

        ushort[] irData;

        byte[] irDataConverted;

        WriteableBitmap irBitmap;
        
        MultiSourceFrameReader msfr;
        private bool StartKinect() {

            kinectSensor = KinectSensor.GetDefault();

            bodies = new Body[6];

            msfr = kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body);

            msfr.MultiSourceFrameArrived += KinectManager_MultiSourceFrameArrived;

            kinectSensor.Open();

            //prepare the hand change timer
            handChangeTimer = new Timer();
            handChangeTimer.Interval = TimeSpan.FromSeconds(handChangeTime).TotalMilliseconds;
            handChangeTimer.Elapsed += HandChangeTimer_Elapsed;
            
            try {
            }
            catch(Exception e) {
                Console.WriteLine("Could not start kinect! E: " + e.Message);
                return false;
            }
            return true;

        }

        private void KinectManager_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame msf = e.FrameReference.AcquireFrame();

            using (BodyFrame body = msf.BodyFrameReference.AcquireFrame()) {
                if (body != null)
                {

                    body.GetAndRefreshBodyData(bodies);

                    Body playerBody = (from b in bodies
                                       where b.IsTracked
                                       select b).FirstOrDefault();

                    if (playerBody != null)
                    {
                        ParseBody(playerBody, (long)body.RelativeTime.TotalMilliseconds);
                    }
                }
            }

        }

        private void HandChangeTimer_Elapsed(object sender, ElapsedEventArgs e) {
            LeftHand = !LeftHand;
        }

        Tuple<long, double> lastPoint = new Tuple<long, double>(0, 0);
        Boolean handClose = false;
        public void ParseBody(Body playerBody, long timeStamp) {

            float center = playerBody.Joints[JointType.SpineShoulder].Position.Y;

            Joint handLeft = playerBody.Joints[JointType.HandTipLeft];
            Joint handRight = playerBody.Joints[JointType.HandTipRight];

            bool t = handLeft.Position.Z < handRight.Position.Z;
            if (LeftHand != t)
            {
                if (!handChangeTimer.Enabled)
                {
                    handChangeTimer.Start();
                }
            }
            else {
                if (handChangeTimer.Enabled)
                {
                    handChangeTimer.Stop();
                }
            }

            Joint pointer = LeftHand ? handLeft : handRight;
            Joint throwHand = LeftHand ? handRight : handLeft;

            var pos = throwHand.Position;
            double distance = Math.Sqrt(Math.Pow((0 - pos.X), 2) + Math.Pow((0 - pos.Y), 2) + Math.Pow((0 - pos.Z), 2));

            Tuple<long, double> currentPoint = new Tuple<long, double>(timeStamp, distance);

            KinectGesture throwGesture = IsThrowing(currentPoint, lastPoint);

            if (throwGesture != null)
            {
                GestureParser.AddKinectGesture(throwGesture);
            }

            lastPoint = currentPoint;


            board.PointAt(pointer.Position.X, pointer.Position.Y - center);
        }

        long lastThrowEvent = 0;

        private KinectGesture IsThrowing(Tuple<long,double> current, Tuple<long,double> last) {

            double distance = current.Item2 - last.Item2;

            if(current.Item1 - lastThrowEvent < 1000) { return null; }

            if(distance < -0.05 && GestureParser.GetDirectionContext() == GestureDirection.Push) {
                Console.WriteLine("Throw Push");
                lastThrowEvent = current.Item1;
                return new KinectGesture(GestureType.Throw, GestureDirection.Push, CanvasWindow.GetCurrentPoint());
            }
            else if(distance > 0.05 && GestureParser.GetDirectionContext() == GestureDirection.Pull) {
                Console.WriteLine("Throw Pull");
                lastThrowEvent = current.Item1;
                return new KinectGesture(GestureType.Throw, GestureDirection.Pull, CanvasWindow.GetCurrentPoint());
            }

            return null;
        }
    }
}
