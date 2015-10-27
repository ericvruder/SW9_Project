using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using System.ComponentModel;
using System.Timers;

namespace SW9_Project {
    class KinectManager {

        KinectSensor kinectSensor;
        string connectedStatus = "";
        IDrawingBoard board;
        private GestureController gestureController;
        Timer _clearTimer;

        double xScale, yScale;

        public KinectManager(IDrawingBoard board) {
            
            this.board = board;

            DiscoverKinectSensor();
            StartKinect();

            _clearTimer = new Timer(2000);
            _clearTimer.Elapsed += new ElapsedEventHandler(clearTimer_Elapsed);
        }

        private bool StartKinect() {

            //kinectSensor.ColorStream.Enable();
            kinectSensor.DepthStream.Range = DepthRange.Near;
            kinectSensor.SkeletonStream.Enable(new TransformSmoothParameters() {
                Smoothing = 0.5f,
                Correction = 0.5f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            });

            // initialize the gesture recognizer
            gestureController = new GestureController();
            gestureController.GestureRecognized += OnGestureRecognized;

            // register the gestures for this demo
            RegisterGestures();

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

                        board.PointAt(rightHand.Position.X, rightHand.Position.Y);
                        gestureController.UpdateAllGestures(playerSkeleton);

                        board.PointAt(rightHand.Position.X, rightHand.Position.Y);
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

        private void RegisterGestures()
        {
            IRelativeGestureSegment[] joinedhandsSegments = new IRelativeGestureSegment[20];
            JoinedHandsSegment1 joinedhandsSegment = new JoinedHandsSegment1();
            for (int i = 0; i < 20; i++)
            {
                // gesture consists of the same thing 10 times 
                joinedhandsSegments[i] = joinedhandsSegment;
            }
            gestureController.AddGesture("JoinedHands", joinedhandsSegments);

            IRelativeGestureSegment[] waveRightSegments = new IRelativeGestureSegment[6];
            WaveRightSegment1 waveRightSegment1 = new WaveRightSegment1();
            WaveRightSegment2 waveRightSegment2 = new WaveRightSegment2();
            waveRightSegments[0] = waveRightSegment1;
            waveRightSegments[1] = waveRightSegment2;
            waveRightSegments[2] = waveRightSegment1;
            waveRightSegments[3] = waveRightSegment2;
            waveRightSegments[4] = waveRightSegment1;
            waveRightSegments[5] = waveRightSegment2;
            gestureController.AddGesture("WaveRight", waveRightSegments);

            IRelativeGestureSegment[] swingRightSegments = new IRelativeGestureSegment[2];
            SwingRightSegment1 swingRightSegment1 = new SwingRightSegment1();
            SwingRightSegment2 swingRightSegment2 = new SwingRightSegment2();
            swingRightSegments[0] = swingRightSegment1;
            swingRightSegments[1] = swingRightSegment2;
            gestureController.AddGesture("SwingRight", swingRightSegments);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void clearTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Gesture = "";
            _clearTimer.Stop();
        }

        private string _gesture;
        public String Gesture
        {
            get { return _gesture; }

            private set
            {
                if (_gesture == value)
                    return;

                _gesture = value;

                if (this.PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Gesture"));
            }
        }

        private void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            switch (e.GestureName)
            {
                case "WaveRight":
                    Gesture = "Wave Right";
                    Console.WriteLine("Wave Right Hand");
                    break;
                case "JoinedHands":
                    Gesture = "Joined Hands";
                    Console.WriteLine("Joined Hands");
                    break;
                case "SwingRight":
                    Gesture = "Swing Right";
                    Console.WriteLine("Swing Right");
                    break;

                default:
                    break;
            }

            _clearTimer.Start();
        }
    }
}
