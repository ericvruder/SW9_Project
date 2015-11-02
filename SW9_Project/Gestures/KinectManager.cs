using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using System.ComponentModel;
using System.Timers;
using SW9_Project.Logging;
using System.Windows;

namespace SW9_Project {
    class KinectManager {

        KinectSensor kinectSensor;
        string connectedStatus = "";
        IDrawingBoard board;
        private GestureController gestureController;
        Logger logger = new Logger();

        public KinectManager(IDrawingBoard board) {
            
            this.board = board;

            DiscoverKinectSensor();
            StartKinect();
            
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
            //gestureController.AddGesture("ThrowPull", ThrowPull);
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private void OnGestureRecognized(object sender, GestureEventArgs e) { 

            Point pointer = board.GetPoint(e.Position.X, e.Position.Y);
            logger.LogGesture(e.GestureName, pointer, e.Time);

            if(e.GestureName == "ThrowPush")
            {
                KinectGesture thrown = new KinectGesture("circle", GestureType.Throw, GestureDirection.Push, pointer);
                GestureParser.AddKinectGesture(thrown);

            }
            else if(e.GestureName == "ThrowPull") {
                KinectGesture pull = new KinectGesture("circle", GestureType.Throw, GestureDirection.Pull, pointer);
                GestureParser.AddKinectGesture(pull);
            }
            
        }
    }
}
