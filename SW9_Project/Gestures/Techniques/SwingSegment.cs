using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW9_Project
{
    public class SwingSegmentBehind : IRelativeGestureSegment {

        JointType pointingHand = JointType.HandLeft, hip = JointType.HipLeft, pointingElbow = JointType.ElbowLeft;
        JointType pointingShoulder = JointType.ShoulderLeft, gestureHand = JointType.HandRight;

        public SwingSegmentBehind(bool leftHandedPointing) {
            if (!leftHandedPointing) {
                pointingHand = JointType.HandRight; hip = JointType.HipRight; pointingElbow = JointType.ElbowRight;
                pointingShoulder = JointType.ShoulderRight; gestureHand = JointType.HandLeft;
            }
        }

        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton.Joints[pointingHand].Position.Y > skeleton.Joints[hip].Position.Y) {
                // If left hand is pointing
                if (skeleton.Joints[pointingHand].Position.Z < skeleton.Joints[pointingElbow].Position.Z)
                {
                    if (skeleton.Joints[pointingElbow].Position.Z < skeleton.Joints[pointingShoulder].Position.Z)
                    {
                        // If right hand is ready to make a gesture towards the screen
                        if (skeleton.Joints[gestureHand].Position.Z > skeleton.Joints[pointingShoulder].Position.Z - 0.1)
                        {
                            return GesturePartResult.Succeed;
                        }
                    }
                    return GesturePartResult.Pausing;
                    // hand has not dropped but is not quite where we expect it to be, pausing till next frame
                }
                return GesturePartResult.Pausing;
            }
            // left hand is not pointing at the screen
            return GesturePartResult.Fail;
        }
    }

    public class SwingSegmentInfront : IRelativeGestureSegment
    {

        JointType pointingHand = JointType.HandLeft, hip = JointType.HipLeft, pointingElbow = JointType.ElbowLeft;
        JointType pointingShoulder = JointType.ShoulderLeft, gestureHand = JointType.HandRight;

        public SwingSegmentInfront(bool leftHandedPointing) {
            if (!leftHandedPointing) {
                pointingHand = JointType.HandRight; hip = JointType.HipRight; pointingElbow = JointType.ElbowRight;
                pointingShoulder = JointType.ShoulderRight; gestureHand = JointType.HandLeft;
            }
        }
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton.Joints[pointingHand].Position.Y > skeleton.Joints[hip].Position.Y)
            {
                // If left hand is pointing
                if (skeleton.Joints[pointingHand].Position.Z < skeleton.Joints[pointingElbow].Position.Z - 0.1)
                {
                    if (skeleton.Joints[pointingElbow].Position.Z < skeleton.Joints[pointingShoulder].Position.Z)
                    {
                        // If right hand is moving towards the screen
                        if (skeleton.Joints[gestureHand].Position.Z < skeleton.Joints[pointingShoulder].Position.Z - 0.20)
                        {
                            return GesturePartResult.Succeed;
                        }
                    }
                    return GesturePartResult.Pausing;
                    // hand has not dropped but is not quite where we expect it to be, pausing till next frame
                }
                return GesturePartResult.Pausing;
            }
            // Left hand is not pointing at the screen
            return GesturePartResult.Fail;
        }
    }
}

