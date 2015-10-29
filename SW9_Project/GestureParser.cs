using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW9_Project {
    static class GestureParser {

        static List<MobileGesture> mobileGestures;
        static public List<MobileGesture> MobileGestures {
            get {
                if(mobileGestures == null) {
                    mobileGestures = new List<MobileGesture>();
                    
                }
                return mobileGestures;
            }
            set {

                if (mobileGestures == null) {
                    mobileGestures = new List<MobileGesture>();

                }
                mobileGestures = value;
            }
        }

        static List<KinectGesture> kinectGestures;

        static public List<KinectGesture> KinectGestures { 
            get {
                if(kinectGestures == null) {
                    kinectGestures = new List<KinectGesture>();
                    
                }
                return kinectGestures;
            }
            set {

                if (kinectGestures == null) {
                    kinectGestures = new List<KinectGesture>();

                }
                kinectGestures = value;
            }
        }

        static public KinectGesture AwaitingGesture { get; set; }

        
        static public void AddMobileGesture(MobileGesture receivedGesture) {
            if (KinectGestures.Count != 0) {
                foreach(var gesture in KinectGestures) {
                    if (receivedGesture.Timestamp - gesture.Timestamp < TimeSpan.FromSeconds(2)) {
                        if (gesture.Type == receivedGesture.Type) {
                            AwaitingGesture = gesture;
                            return;
                        }
                    }
                }

            } 
            MobileGestures.Add(receivedGesture);
        }

        static public void AddKinectGesture(KinectGesture receivedGesture) {
            if(MobileGestures.Count != 0) {
                foreach(var gesture in MobileGestures) {
                    if (receivedGesture.Timestamp - gesture.Timestamp < TimeSpan.FromSeconds(2)) {
                        if (gesture.Type == receivedGesture.Type) {
                            AwaitingGesture = receivedGesture;
                            return;
                        }
                    } 
                }
            }
            KinectGestures.Add(receivedGesture);
        }
    }
}
