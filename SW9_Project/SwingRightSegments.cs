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
            // hand above elbow
            if (skeleton.Joints[JointType.HandRight].Position.Z > skeleton.Joints[JointType.Spine].Position.Z)
            {
                   return GesturePartResult.Succeed;
            }
            // hand has not dropped but is not quite where we expect it to be, pausing till next frame
            return GesturePartResult.Fail;
        }
    }

    public class SwingRightSegment2 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // hand above elbow
            if (skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.Spine].Position.Z)
            {
                return GesturePartResult.Succeed;
            }
            // hand has not dropped but is not quite where we expect it to be, pausing till next frame
            return GesturePartResult.Fail;
        }
    }
}

