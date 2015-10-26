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
using System.ComponentModel;
using System.Timers;

namespace SW9_Project {
    class KinectManager {

        KinectSensor kinectSensor;
        string connectedStatus = "";
        IDrawingBoard board;
        private GestureController gestureController;
        Timer _clearTimer;

        public KinectManager(IDrawingBoard board) {
            this.board = board;
            DiscoverKinectSensor();
            Calibrate();

            _clearTimer = new Timer(2000);
            _clearTimer.Elapsed += new ElapsedEventHandler(clearTimer_Elapsed);
        }

        private void Calibrate() {
            board.SetScalingFactor(2050.0, 1000.0); //TODO: Create a calibration routine
        }

        private bool StartKinect() {

            kinectSensor.ColorStream.Enable();
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

        protected static int origRow;
        protected static int origCol;
        protected static void WriteAt(string s, int x, int y)
        {
            try
            {
                Console.SetCursorPosition(origCol + x, origRow + y);
                Console.Write(s);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }

        private void KinectSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e) {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) {
                if (skeletonFrame != null) {
                    Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    Skeleton playerSkeleton = (from s in skeletonData where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                    if (playerSkeleton != null) {
                        Joint rightHand = playerSkeleton.Joints[JointType.HandRight];
                        Joint leftHand = playerSkeleton.Joints[JointType.HandLeft];
                        Joint AnkleLeft = playerSkeleton.Joints[JointType.AnkleLeft];
                        Joint AnkleRight = playerSkeleton.Joints[JointType.AnkleRight];
                        Joint ElbowLeft = playerSkeleton.Joints[JointType.ElbowLeft];
                        Joint ElbowRight = playerSkeleton.Joints[JointType.ElbowRight];
                        Joint FootLeft = playerSkeleton.Joints[JointType.FootLeft];
                        Joint FootRight = playerSkeleton.Joints[JointType.FootRight];
                        Joint Head = playerSkeleton.Joints[JointType.Head];
                        Joint HipCenter = playerSkeleton.Joints[JointType.HipCenter];
                        Joint HipLeft = playerSkeleton.Joints[JointType.HipLeft];
                        Joint HipRight = playerSkeleton.Joints[JointType.HipRight];
                        Joint KneeLeft = playerSkeleton.Joints[JointType.KneeLeft];
                        Joint KneeRight = playerSkeleton.Joints[JointType.KneeRight];
                        Joint ShoulderCenter = playerSkeleton.Joints[JointType.ShoulderCenter];
                        Joint ShoulderLeft = playerSkeleton.Joints[JointType.ShoulderLeft];
                        Joint ShoulderRight = playerSkeleton.Joints[JointType.ShoulderRight];
                        Joint Spine = playerSkeleton.Joints[JointType.Spine];
                        Joint WristLeft = playerSkeleton.Joints[JointType.WristLeft];
                        Joint WristRight = playerSkeleton.Joints[JointType.WristRight];

                        //Console.WriteLine(rightHand.Position.X + ", " + rightHand.Position.Y);

                        Console.Clear();
                        WriteAt("Hand Right \t X: " + rightHand.Position.X + "\t Y: " + rightHand.Position.Y + "\t Z: " + rightHand.Position.Z, 0, 0);
                        WriteAt("Hand Left \t X: " + leftHand.Position.X + "\t Y: " + leftHand.Position.Y + "\t Z: " + leftHand.Position.Z, 0, 1);
                        WriteAt("Ankle Right \t X: " + AnkleRight.Position.X + "\t Y: " + AnkleRight.Position.Y + "\t Z: " + AnkleRight.Position.Z, 0, 2);
                        WriteAt("Ankle Left \t X: " + AnkleLeft.Position.X + "\t Y: " + AnkleLeft.Position.Y + "\t Z: " + AnkleLeft.Position.Z, 0, 3);
                        WriteAt("Elbow Right \t X: " + ElbowRight.Position.X + "\t Y: " + ElbowRight.Position.Y + "\t Z: " + ElbowRight.Position.Z, 0, 4);
                        WriteAt("Elbow Left \t X: " + ElbowLeft.Position.X + "\t Y: " + ElbowLeft.Position.Y + "\t Z: " + ElbowLeft.Position.Z, 0, 5);
                        WriteAt("Foot Right \t X: " + FootRight.Position.X + "\t Y: " + FootRight.Position.Y + "\t Z: " + FootRight.Position.Z, 0, 6);
                        WriteAt("Foot Left \t X: " + FootLeft.Position.X + "\t Y: " + FootLeft.Position.Y + "\t Z: " + FootLeft.Position.Z, 0, 7);
                        WriteAt("Head \t\t X: " + Head.Position.X + "\t Y: " + Head.Position.Y + "\t Z: " + Head.Position.Z, 0, 8);
                        WriteAt("Hip Center \t X: " + HipCenter.Position.X + "\t Y: " + HipCenter.Position.Y + "\t Z: " + HipCenter.Position.Z, 0, 9);
                        WriteAt("Hip Right \t X: " + HipRight.Position.X + "\t Y: " + HipRight.Position.Y + "\t Z: " + HipRight.Position.Z, 0, 10);
                        WriteAt("Hip Left \t X: " + HipLeft.Position.X + "\t Y: " + HipLeft.Position.Y + "\t Z: " + HipLeft.Position.Z, 0, 11);
                        WriteAt("Knee Right \t X: " + KneeRight.Position.X + "\t Y: " + KneeRight.Position.Y + "\t Z: " + KneeRight.Position.Z, 0, 12);
                        WriteAt("Knee Left \t X: " + KneeLeft.Position.X + "\t Y: " + KneeLeft.Position.Y + "\t Z: " + KneeLeft.Position.Z, 0, 13);
                        WriteAt("Shoulder Center  X: " + ShoulderCenter.Position.X + "\t Y: " + ShoulderCenter.Position.Y + "\t Z: " + ShoulderCenter.Position.Z, 0, 14);
                        WriteAt("Shoulder Right \t X: " + ShoulderRight.Position.X + "\t Y: " + ShoulderRight.Position.Y + "\t Z: " + ShoulderRight.Position.Z, 0, 15);
                        WriteAt("Shoulder Left \t X: " + ShoulderLeft.Position.X + "\t Y: " + ShoulderLeft.Position.Y + "\t Z: " + ShoulderLeft.Position.Z, 0, 16);
                        WriteAt("Spine \t\t X: " + Spine.Position.X + "\t Y: " + Spine.Position.Y + "\t Z: " + Spine.Position.Z, 0, 17);
                        WriteAt("Wrist Right \t X: " + WristRight.Position.X + "\t Y: " + WristRight.Position.Y + "\t Z: " + WristRight.Position.Z, 0, 18);
                        WriteAt("Wrist Left \t X: " + WristLeft.Position.X + "\t Y: " + WristLeft.Position.Y + "\t Z: " + WristLeft.Position.Z, 0, 19);

                        board.PointAt(rightHand.Position.X, rightHand.Position.Y);
                        gestureController.UpdateAllGestures(playerSkeleton);
                        //handPosition = new Vector2((((0.5f * rightHand.Position.X) + 0.5f) * (640)), (((-0.5f * rightHand.Position.Y) + 0.5f) * (480)));
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

            // You can use the kinectSensor.Status to check for status
            // and give the user some kind of feedback
            switch (kinectSensor.Status) {
                case KinectStatus.Connected:
                    {
                        connectedStatus = "Status: Connected";
                        break;
                    }
                case KinectStatus.Disconnected:
                    {
                        connectedStatus = "Status: Disconnected";
                        break;
                    }
                case KinectStatus.NotPowered:
                    {
                        connectedStatus = "Status: Connect the power";
                        break;
                    }
                default:
                    {
                        connectedStatus = "Status: Error";
                        break;
                    }
            }

            // Init the found and connected device
            if (kinectSensor.Status == KinectStatus.Connected) {
                StartKinect();
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

        private void KinectSensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e) {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) {
                if (skeletonFrame != null) {
                    Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    Skeleton playerSkeleton = (from s in skeletonData where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                    if (playerSkeleton != null) {
                        Joint rightHand = playerSkeleton.Joints[JointType.HandRight];
                        //Console.WriteLine(rightHand.Position.X + ", " + rightHand.Position.Y);
                        //handPosition = new Vector2((((0.5f * rightHand.Position.X) + 0.5f) * (640)), (((-0.5f * rightHand.Position.Y) + 0.5f) * (480)));
                    }
                }
            }
        }

    }


}
