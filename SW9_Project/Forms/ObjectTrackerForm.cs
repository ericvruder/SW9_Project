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

        CaptureDeviceManager captureManager = new CaptureDeviceManager();
        bool blnCapturingInProcess = false;
        ShapeDetection detector = new ShapeDetection();

        private void ObjectTrackerForm_Load(object sender, EventArgs e) {
            captureManager.InitializeCaptureDevices();
            Application.Idle += ProcessFrameAndUpdateGUI;
            blnCapturingInProcess = true;
        }

        private void ProcessFrameAndUpdateGUI(object sender, EventArgs e) {

            Image<Bgr, byte> image = captureManager.GetNextFrame();

            ibOriginal.Image = image.Mat;
            ibThresh.Image = detector.DetectShapes(image);
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
