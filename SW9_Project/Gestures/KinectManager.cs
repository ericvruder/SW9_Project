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
        IDrawingBoard board;
        private GestureController gestureController;
        private InteractionStream _interactionStream;
        private UserInfo[] userInfos;

        private Timer handChangeTimer;
        private double handChangeTime = 2; //seconds to wait for hand change

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
            //kinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            if (kinectSensor == null)
                return false;

            kinectSensor.SkeletonStream.Enable(new TransformSmoothParameters() {
                Smoothing = 0.7f,
                Correction = 0.3f,
                Prediction = 1.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 1.0f
            });

            // initialize the gesture recognizer
            gestureController = new GestureController();
            gestureController.GestureRecognized += OnGestureRecognized;

            //prepare the hand change timer
            handChangeTimer = new Timer();
            handChangeTimer.Interval = TimeSpan.FromSeconds(handChangeTime).TotalMilliseconds;
            handChangeTimer.Elapsed += HandChangeTimer_Elapsed;

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

        private void HandChangeTimer_Elapsed(object sender, ElapsedEventArgs e) {
            LeftHand = !LeftHand;
            RegisterGestures(LeftHand);
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
                    
                    var skeletons = (from s in skeletonData
                                     where s.TrackingState == SkeletonTrackingState.Tracked
                                     select s);
                    Skeleton playerSkeleton = (from t in skeletons
                                               where t.Joints[JointType.Head].Position.Z == skeletons.Min(x => x.Joints[JointType.Head].Position.Z)
                                               select t).FirstOrDefault();
                    if (playerSkeleton != null) {


                        float center = playerSkeleton.Joints[JointType.ShoulderCenter].Position.Y;
                        
                        Joint HandLeft = playerSkeleton.Joints[JointType.HandLeft];
                        Joint HandRight = playerSkeleton.Joints[JointType.HandRight];

                        bool t = HandLeft.Position.Z < HandRight.Position.Z;
                        if (LeftHand != t) {
                            if (!handChangeTimer.Enabled) {
                                handChangeTimer.Start();
                            }
                        }
                        else {
                            if (handChangeTimer.Enabled) {
                                handChangeTimer.Stop();
                            }
                        }


                        Joint Pointer = LeftHand ? HandLeft : HandRight;
                        gestureController.UpdateAllGestures(playerSkeleton);

                        board.PointAt(Pointer.Position.X, Pointer.Position.Y - center); 

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
        }

        private void RegisterGestures(bool leftHand)
        {
            gestureController.ClearAllGestures();
            IRelativeGestureSegment[] ThrowPush = new IRelativeGestureSegment[2];
            IRelativeGestureSegment[] ThrowPull = new IRelativeGestureSegment[2];


            ThrowPush[0] = new SwingSegmentBehind(leftHand);
            ThrowPush[1] = new SwingSegmentInfront(leftHand);

            ThrowPull[0] = new SwingSegmentInfront(leftHand);
            ThrowPull[1] = new SwingSegmentBehind(leftHand);

            gestureController.AddGesture("ThrowPush", ThrowPush);
            gestureController.AddGesture("ThrowPull", ThrowPull);
        }

        private void OnGestureRecognized(object sender, GestureEventArgs e) { 

            Point pointer = board.GetPoint(e.Position.X, e.Position.Y);

            if(e.GestureName == "ThrowPush")
            {
                Console.WriteLine("Throw Push");
                KinectGesture thrown = new KinectGesture(GestureType.Throw, GestureDirection.Push, pointer);
                GestureParser.AddKinectGesture(thrown);

            }
            else if(e.GestureName == "ThrowPull") {
                Console.WriteLine("Throw Pull");
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
                                Console.WriteLine("Release");
                                KinectGesture kinectGesture = new KinectGesture(null, GestureType.Pinch, GestureDirection.Push, CanvasWindow.GetCurrentPoint());
                                GestureParser.AddKinectGesture(kinectGesture);
                            }
                            else
                            {
                                Console.WriteLine("Grip");
                                KinectGesture kinectGesture = new KinectGesture(null, GestureType.Pinch, GestureDirection.Pull, CanvasWindow.GetCurrentPoint());
                                GestureParser.AddKinectGesture(kinectGesture);
                            }
                        }
                    }
                }
            }
        }
    }
}
