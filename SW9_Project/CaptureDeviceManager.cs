using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SW9_Project {
    class CaptureDeviceManager {

        KinectSensor kinectSensor;
        Capture capWebcam;

         public bool InitializeCaptureDevices() {
            try {
                capWebcam = new Capture();
                kinectSensor = KinectSensor.KinectSensors[0];
                kinectSensor.ColorStream.Enable();
                kinectSensor.DepthStream.Enable();
                kinectSensor.Start();
                return true;
            } catch (Exception ex) {
                MessageBox.Show("unable to read from cam, error: " + Environment.NewLine + Environment.NewLine +
                                ex.Message + Environment.NewLine + Environment.NewLine +
                                "exiting program");
                Environment.Exit(0);
                return false;
            }
        }

        public Image<Bgr, byte> GetNextFrame() {
            using (ColorImageFrame receivedFrame = kinectSensor.ColorStream.OpenNextFrame(0)) {
                return new Image<Bgr, byte>(ImageConverter.ImageToBitmap(receivedFrame));
            }

        }
    }
}
