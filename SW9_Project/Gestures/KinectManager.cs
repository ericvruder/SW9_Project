using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using System.ComponentModel;
using System.Timers;
using SW9_Project.Logging;
using System.Windows;
using Microsoft.Kinect.Toolkit.Interaction;
using SW9_Project.Gestures;

namespace SW9_Project {
    class KinectManager {

        KinectSensor kinectSensor;
        string connectedStatus = "";
        IDrawingBoard board;
        private GestureController gestureController;
        Timer _clearTimer;
        Logger logger = new Logger();
        private InteractionStream _interactionStream;
        private UserInfo[] userInfos;

        public KinectManager(IDrawingBoard board) {
            
            this.board = board;

            DiscoverKinectSensor();
            StartKinect();

            _clearTimer = new Timer(2000);
            _clearTimer.Elapsed += new ElapsedEventHandler(clearTimer_Elapsed);
        }

        private bool StartKinect() {

            //kinectSensor.ColorStream.Enable();
            //kinectSensor.DepthStream.Range = DepthRange.Near;
            kinectSensor.DepthStream.Enable();
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

            userInfos = new UserInfo[InteractionFrame.UserInfoArrayLength];
            _interactionStream = new InteractionStream(kinectSensor, new InteractionClient());
            _interactionStream.InteractionFrameReady += InteractionStreamOnInteractionFrameReady;
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
                    _interactionStream.ProcessSkeleton(skeletonData, kinectSensor.AccelerometerGetCurrentReading(), skeletonFrame.Timestamp);
                    
                    Skeleton playerSkeleton = (from s in skeletonData where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                    if (playerSkeleton != null) {
                        
                        Joint HandLeft = playerSkeleton.Joints[JointType.HandLeft];
                        //Joint HandRight = playerSkeleton.Joints[JointType.HandRight];
                        //Joint HipLeft = playerSkeleton.Joints[JointType.HipLeft];
                        //Console.Clear();
                        //Console.Write(
                        //    "HandLeft: x: "+HandLeft.Position.X+" y:"+HandLeft.Position.Y+" z:"+HandLeft.Position.Z+"\n" +
                        //    "HipLeft: x: " + HipLeft.Position.X + " y:" + HipLeft.Position.Y + " z:" + HipLeft.Position.Z + "\n"
                        //    );

                        gestureController.UpdateAllGestures(playerSkeleton);
                        board.PointAt(HandLeft.Position.X, HandLeft.Position.Y);
                    }
                }
            }

            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if(depthFrame != null)
                {
                    _interactionStream.ProcessDepth(depthFrame.GetRawPixelData(), depthFrame.Timestamp);
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
            IRelativeGestureSegment[] ThrowPush = new IRelativeGestureSegment[2];
            ThrowPush[0] = new SwingRightSegment1();
            ThrowPush[1] = new SwingRightSegment2();
            gestureController.AddGesture("ThrowPush", ThrowPush);

            IRelativeGestureSegment[] ThrowPull = new IRelativeGestureSegment[2];
            ThrowPull[0] = new SwingRightSegment2();
            ThrowPull[1] = new SwingRightSegment1();
            gestureController.AddGesture("ThrowPull", ThrowPull);
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

        private void OnGestureRecognized(object sender, GestureEventArgs e) { 

            Point pointer = board.GetPoint(e.Position.X, e.Position.Y);
            logger.LogKinectGesture(e.GestureName, pointer, e.Time);

            if(e.GestureName == "ThrowPush")
            {
                KinectGesture thrown = new KinectGesture("circle", GestureType.Throw, GestureDirection.Push, pointer);
                GestureParser.AddKinectGesture(thrown);

            }
            else if(e.GestureName == "ThrowPull") {
                KinectGesture pull = new KinectGesture("circle", GestureType.Throw, GestureDirection.Pull, pointer);
                GestureParser.AddKinectGesture(pull);
            }

            _clearTimer.Start();
        }

        private void InteractionStreamOnInteractionFrameReady(object sender, InteractionFrameReadyEventArgs e)
        {
            using (InteractionFrame frame = e.OpenInteractionFrame())
            {
                if (frame != null)
                {
                    if (this.userInfos == null)
                    {
                        this.userInfos = new UserInfo[InteractionFrame.UserInfoArrayLength];
                    }

                    frame.CopyInteractionDataTo(this.userInfos);
                }
                else
                {
                    return;
                }
            }



            foreach (UserInfo userInfo in this.userInfos)
            {
                foreach (InteractionHandPointer handPointer in userInfo.HandPointers)
                {
                    string action = null;

                    switch (handPointer.HandEventType)
                    {
                        case InteractionHandEventType.Grip:
                            action = "gripped";
                            break;

                        case InteractionHandEventType.GripRelease:
                            action = "released";

                            break;
                    }

                    if (action != null)
                    {
                        string handSide = "unknown";

                        switch (handPointer.HandType)
                        {
                            case InteractionHandType.Left:
                                handSide = "left";
                                break;

                            case InteractionHandType.Right:
                                handSide = "right";
                                break;
                        }

                        if (handSide == "left")
                        {
                            if (action == "released")
                            {
                                // left hand released code here
                                Console.WriteLine("Left hand release");
                            }
                            else
                            {
                                // left hand gripped code here
                                Console.WriteLine("Left hand grip");
                            }
                        }
                        else
                        {
                            if (action == "released")
                            {
                                // right hand released code here
                                Console.WriteLine("Right hand release");
                            }
                            else
                            {
                                // right hand gripped code here
                                Console.WriteLine("Right hand grip");
                            }
                        }
                    }
                }
            }
        }
    }
}
