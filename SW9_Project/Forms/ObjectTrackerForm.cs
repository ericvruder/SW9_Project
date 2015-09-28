using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;

using Microsoft.Kinect;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SW9_Project {
    public partial class ObjectTrackerForm : Form {
        public ObjectTrackerForm() {
            InitializeComponent();
        }

        Capture capWebcam;
        KinectSensor kinectRGBSensor;
        bool blnCapturingInProcess = false;

        private void ObjectTrackerForm_Load(object sender, EventArgs e) {
            InitializeCaptureDevices();
            Application.Idle += NextFrame;
            blnCapturingInProcess = true;
        }

        //private void NextFrame(object sender, EventArgs e) {
        //    using (ColorImageFrame receivedFrame = kinectRGBSensor.ColorStream.OpenNextFrame(0)) {
        //        if (receivedFrame != null) {
        //            processFrameAndUpdateGUI(new Image<Bgr, byte>(ImageConverter.ImageToBitmap(receivedFrame)));
        //        }

        //    }
        //}
        private void NextFrame(object sender, EventArgs e)
        {
            using (DepthImageFrame receivedFrame = kinectRGBSensor.DepthStream.OpenNextFrame(0))
            {
                if (receivedFrame != null)
                {
                    processFrameAndUpdateGUI(new Image<Bgr, byte>(ImageConverter.ImageToBitmap(receivedFrame)));
                }

            }
        }

        private bool InitializeCaptureDevices() {
            try {
                capWebcam = new Capture();
                kinectRGBSensor = KinectSensor.KinectSensors[0];
                kinectRGBSensor.ColorStream.Enable();
                kinectRGBSensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30)
                kinectRGBSensor.Start();
                return true;
            } catch (Exception ex) {
                MessageBox.Show("unable to read from cam, error: " + Environment.NewLine + Environment.NewLine +
                                ex.Message + Environment.NewLine + Environment.NewLine +
                                "exiting program");
                Environment.Exit(0);
                return false;
            }
        }


        ShapeDetection detector = new ShapeDetection();
        void processFrameAndUpdateGUI(Image<Bgr, byte> frame) {

            ibOriginal.Image = frame.Mat;
            ibThresh.Image = detector.DetectShapes(frame);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnPauseOrResume_Click(object sender, EventArgs e) {
            if (blnCapturingInProcess == true) {                    // if we are currently processing an image, user just choose pause, so . . .
                //Application.Idle -= processFrameAndUpdateGUI;       // remove the process image function from the application's list of tasks
                blnCapturingInProcess = false;                      // update flag variable
                btnPauseOrResume.Text = " Resume ";                 // update button text
            } else {                                                // else if we are not currently processing an image, user just choose resume, so . . .
                //Application.Idle += processFrameAndUpdateGUI;       // add the process image function to the application's list of tasks
                blnCapturingInProcess = true;                       // update flag variable
                btnPauseOrResume.Text = " Pause ";                  // new button will offer pause option
            }
        }
    }
}
