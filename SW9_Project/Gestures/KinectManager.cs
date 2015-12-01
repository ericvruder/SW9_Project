using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using Microsoft.Kinect.Toolkit.Interaction;
using SW9_Project.Gestures;
using SW9_Project.Logging;

namespace SW9_Project {
    class KinectManager {

        KinectSensor kinectSensor;
        string connectedStatus = "";
        IDrawingBoard board;
        private GestureController gestureController;
        private InteractionStream _interactionStream;
        private UserInfo[] userInfos;
        public event PropertyChangedEventHandler PropertyChanged;

        public KinectManager(IDrawingBoard board) {
            
            this.board = board;

            DiscoverKinectSensor();
            StartKinect();
            
        }

        public void RaiseAngle(int angleChange)
        {
            try
            {
                kinectSensor.ElevationAngle += angleChange;
            }
            catch { }
        }

        public void LowerAngle(int  angleChange)
        {
            try
            { 
                kinectSensor.ElevationAngle -= angleChange;
            } catch { }
        }

        private bool LeftHand = true;

        private bool StartKinect() {

            //kinectSensor.ColorStream.Enable();
            //kinectSensor.DepthStream.Range = DepthRange.Near;
            if (kinectSensor == null)
                return false;
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
            RegisterGestures(LeftHand);

            userInfos = new UserInfo[InteractionFrame.UserInfoArrayLength];
            _interactionStream = new InteractionStream(kinectSensor, new InteractionClient());
            _interactionStream.InteractionFrameReady += InteractionStreamOnInteractionFrameReady;
            kinectSensor.AllFramesReady += KinectSensor_AllFramesReady;
            try {
                kinectSensor.Start();
                kinectSensor.ElevationAngle = 18;
            }
            catch(Exception e) {
                Console.WriteLine("Could not start kinect! E: " + e.Message);
                return false;
            }
            return true;

        }

        /// <summary>
        /// This event is fired when all frames of the Kinect are ready.
        /// This includes depth, color and skeleton frames.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KinectSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e) {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) {
                if (skeletonFrame != null) {
                    Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    _interactionStream.ProcessSkeleton(skeletonData, kinectSensor.AccelerometerGetCurrentReading(), skeletonFrame.Timestamp);
                    
                    Skeleton playerSkeleton = (from s in skeletonData where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                    if (playerSkeleton != null) {
                        
                        Joint HandLeft = playerSkeleton.Joints[JointType.HandLeft];
                        Joint HandRight = playerSkeleton.Joints[JointType.HandRight];

                        bool t = HandLeft.Position.Z < HandRight.Position.Z;
                        if (LeftHand != t) {
                            LeftHand = t;
                            RegisterGestures(LeftHand);
                        }


                        Joint Pointer = LeftHand ? HandLeft : HandRight;
                        gestureController.UpdateAllGestures(playerSkeleton);

                        // Left handed
                        board.PointAt(Pointer.Position.X, Pointer.Position.Y); // This is used for the throw technique
                        //board.PointAt(HandRight.Position.X, HandRight.Position.Y); // This is used for all other techniques

                        // Right handed
                        //board.PointAt(HandRight.Position.X, HandRight.Position.Y); // This is used for the throw technique
                        //board.PointAt(HandLeft.Position.X, HandLeft.Position.Y); // This is used for all other techniques

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

        private void RegisterGestures(bool leftHand)
        {
            gestureController.ClearAllGestures();
            IRelativeGestureSegment[] ThrowPush = new IRelativeGestureSegment[2];
            IRelativeGestureSegment[] ThrowPull = new IRelativeGestureSegment[2];

            if (leftHand) {
                ThrowPush[0] = new SwingRightSegment1();
                ThrowPush[1] = new SwingRightSegment2();
            } else {
                ThrowPush[0] = new SwingLeftSegment1();
                ThrowPush[1] = new SwingLeftSegment2();
            }

            gestureController.AddGesture("ThrowPush", ThrowPush);
        }

        private void OnGestureRecognized(object sender, GestureEventArgs e) { 

            Point pointer = board.GetPoint(e.Position.X, e.Position.Y);

            if(e.GestureName == "ThrowPush")
            {
                KinectGesture thrown = new KinectGesture(GestureType.Throw, GestureDirection.Push, pointer);
                GestureParser.AddKinectGesture(thrown);

            }
            else if(e.GestureName == "ThrowPull") {
                KinectGesture pull = new KinectGesture(GestureType.Throw, GestureDirection.Pull, pointer);
                GestureParser.AddKinectGesture(pull);
            }
            
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

                        string correct = LeftHand ? "left" : "right";
                        if (handSide == correct)
                        {
                            if (action == "released")
                            {
                                KinectGesture kinectGesture = new KinectGesture(null, GestureType.Pinch, GestureDirection.Push, CanvasWindow.GetCurrentPoint());
                                GestureParser.AddKinectGesture(kinectGesture);
                            }
                            else
                            {
                                KinectGesture kinectGesture = new KinectGesture(null, GestureType.Pinch, GestureDirection.Pull, CanvasWindow.GetCurrentPoint());
                                GestureParser.AddKinectGesture(kinectGesture);
                            }
                        }
                        else
                        {
                            if (action == "released")
                            {
                                // right hand released code here
                                //Console.WriteLine("Right hand release");
                            }
                            else
                            {
                                // right hand gripped code here
                                //Console.WriteLine("Right hand grip");
                            }
                        }
                    }
                }
            }
        }
    }
}
