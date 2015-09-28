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

            try {

                Image<Bgr, byte> image = captureManager.GetNextFrame();

                ibOriginal.Image = image.Mat;
                ibThresh.Image = detector.DetectShapes(image);
                
                /*//test IR idea
                Image <Gray,Byte> grayImage = image.Convert<Gray, Byte>();
                ibOriginal.Image = AmplifyLow(grayImage);
                ibThresh.Image = AmplifyHigh(grayImage);
                */
            }
            catch(Exception) {

            }
        }

        public Image<Bgr, byte> ToGrayscale(Image<Bgr,byte> image)
        {
            /* Use this instead!!!
             * Image <Bgr,Byte> ColordImage = cap.QueryFrame();
             * Image <Gray,Byte> grayImage = ColordImage.Convert<Gray, Byte>();
             * imageBox1.Image = grayImage;
             */
            Image<Bgr, byte> image2 = image;
            for (int y = 0; y < image2.Size.Height; y++)
			{
                for (int x = 0; x < image2.Size.Width; x++)
			    {
			        double b = image2[y,x].Blue;
                    double g = image2[y,x].Green;
                    double r = image2[y,x].Red;
                    double avg = (b+g+r) / 3;
                    image2[y,x] = new Bgr(avg,avg,avg);
			    }
			}
            return image2;
        }

        public Image<Gray,byte> AmplifyLow(Image<Gray,byte> image)
        {
            Image<Gray, byte> image2 = image;
            for (int y = 0; y < image2.Size.Height; y++)
            {
                for (int x = 0; x < image2.Size.Width; x++)
                {
                    //Low-pass filter
                    //everything above 128 is ignored. To us that means it is 255
                    if (image2[y,x].Intensity >= 128)
                    {
                        image2[y, x] = new Gray(255);
                    }
                    else
                    {
                        image2[y, x] = new Gray(image2[y, x].Intensity * 2);  //amplifying to make 128 bits strech across 256.
                    }
                }
            }
            return image2;

        }

        public Image<Gray, byte> AmplifyHigh(Image<Gray, byte> image)
        {
            Image<Gray, byte> image2 = image;
            for (int y = 0; y < image2.Size.Height; y++)
            {
                for (int x = 0; x < image2.Size.Width; x++)
                {
                    //High-pass filter
                    //everything below 128 is ignored. To us that means it is 0
                    if (image2[y, x].Intensity <= 128)
                    {
                        image2[y, x] = new Gray(0);
                    }
                    else
                    {
                        image2[y, x] = new Gray((image2[y, x].Intensity - 128  ) * 2);  //amplifying to make 128 bits strech across 256.
                    }
                }
            }
            return image2;

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
