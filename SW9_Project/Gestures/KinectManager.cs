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
        KinectJointFilter filter;

        private Timer handChangeTimer;
        private double handChangeTime = 2; //seconds to wait for hand change

        public KinectManager(IDrawingBoard board) {
            
            this.board = board;
            filter = new KinectJointFilter();
            StartKinect();
            
        }

        private bool LeftHand = true;
        private HandState currentHandState = HandState.Open;
        private KinectGesture handGesture;
        
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

        private bool HandStateChanged(HandState handstate) {
            if(handstate == HandState.Unknown) { return false; }
            if(handstate != currentHandState) {
                if(handstate == HandState.Open)
                {
                    currentHandState = handstate;
                    Console.WriteLine("Opened");
                    handGesture = new KinectGesture(GestureType.Pinch, GestureDirection.Pull);
                    return true;
                } else if (handstate == HandState.Closed)
                {
                    currentHandState = handstate;
                    Console.WriteLine("Closed");
                    handGesture = new KinectGesture(GestureType.Pinch, GestureDirection.Push);
                    return true;
                }
                return false;
            }
            return false;
        }

        private void HandChangeTimer_Elapsed(object sender, ElapsedEventArgs e) {
            LeftHand = !LeftHand;
            currentHandState = HandState.Unknown;
        }

        Tuple<long, double> lastPoint = new Tuple<long, double>(0, 0);
        public void ParseBody(Body playerBody, long timeStamp) {

            filter.UpdateFilter(playerBody);
            var joints = filter.GetFilteredJoints();

            float center = joints[(int)JointType.SpineShoulder].Y;

            var handLeft = joints[(int)JointType.HandTipLeft];
            var handRight = joints[(int)JointType.HandTipRight];

            bool t = handLeft.Z < handRight.Z;
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

            var pointer = LeftHand ? handLeft : handRight;
            var throwHand = LeftHand ? handRight : handLeft;
            HandState handState = LeftHand ? playerBody.HandLeftState : playerBody.HandRightState;

            var pos = throwHand;
            double distance = Math.Sqrt(Math.Pow((0 - pos.X), 2) + Math.Pow((0 - pos.Y), 2) + Math.Pow((0 - pos.Z), 2));

            Tuple<long, double> currentPoint = new Tuple<long, double>(timeStamp, distance);

            KinectGesture throwGesture = IsThrowing(currentPoint, lastPoint);

            if (throwGesture != null)
            {
                GestureParser.AddKinectGesture(throwGesture);
            }

            lastPoint = currentPoint;

            if (HandStateChanged(handState)) {
                GestureParser.AddKinectGesture(handGesture);
            }
            board.PointAt(pointer.X, pointer.Y - center);
        }

        long lastThrowEvent = 0;

        private KinectGesture IsThrowing(Tuple<long,double> current, Tuple<long,double> last) {

            double distance = current.Item2 - last.Item2;

            if(current.Item1 - lastThrowEvent < 1000) { return null; }

            if(distance < -0.05 && GestureParser.GetDirectionContext() == GestureDirection.Push) {
                Console.WriteLine("Throw Push");
                lastThrowEvent = current.Item1;
                return new KinectGesture(GestureType.Throw, GestureDirection.Push);
            }
            else if(distance > 0.05 && GestureParser.GetDirectionContext() == GestureDirection.Pull) {
                Console.WriteLine("Throw Pull");
                lastThrowEvent = current.Item1;
                return new KinectGesture(GestureType.Throw, GestureDirection.Pull);
            }

            return null;
        }
    }
}
