using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW9_Project
{
    public class SwingRightSegment1 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipLeft].Position.Y) {
                // If left hand is pointing
                if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z)
                {
                    if (skeleton.Joints[JointType.ElbowLeft].Position.Z < skeleton.Joints[JointType.ShoulderLeft].Position.Z)
                    {
                        // If right hand is ready to make a gesture towards the screen
                        if (skeleton.Joints[JointType.HandRight].Position.Z > skeleton.Joints[JointType.ShoulderLeft].Position.Z + 0.1)
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

    public class SwingRightSegment2 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipLeft].Position.Y)
            {
                // If left hand is pointing
                if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z)
                {
                    if (skeleton.Joints[JointType.ElbowLeft].Position.Z < skeleton.Joints[JointType.ShoulderLeft].Position.Z)
                    {
                        // If right hand is moving towards the screen
                        if (skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ShoulderLeft].Position.Z - 0.1)
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

