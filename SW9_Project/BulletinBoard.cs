using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SW9_Project {

    class ImageContainer
    {
        private List<System.Windows.Controls.Image> ImageList;
        private int pos; // Queue, max 99
        public ImageContainer()
        {
            ImageList = new List<System.Windows.Controls.Image>();
            pos = 0;
        }
        //it is assumed that the image being added here is using a transformedbitmap as soruce
        // and that the source has been verified though those methods.
        public void AddImage(System.Windows.Controls.Image i)
        {
            //Allow for max 100
            if (pos >= 99)
            {
                pos = 0;
            }
            if (ImageList[pos] != null)
            {
                //Clear the image in this position. - probably overkill
                ImageList[pos].Source = null;
                ImageList[pos] = null;
            }
            ImageList[pos] = i;
        }
        public void ClearList()
        { ImageList.Clear(); }
    }

    class BulletinBoard : CanvasWindow {

        ImageContainer imgContainer;
        /// <summary>
        /// I skal hold styr på background, om det skal være et billede eller farve skal ordnes ellers forsvinder griddet ikke
        /// At kalde base med false fortæller canvas window at det er en bulletin board der arbejder, den sætter variable targetPractice = false
        /// så bare bruge det hvis i skal ændre nogen ting i canvaswindow.cs
        /// </summary>
        public BulletinBoard() : base(false) {
            canvas.Background = Brushes.Gray;
            imgContainer = new ImageContainer();
            
        }

        /// <summary>
        /// This gets called every draw cycle, IE every time the kinect captures a frame. 
        /// </summary>
        /// <param name="gesture">Any gestures that were captured. Can be null</param>
        //public override void ExtendedDraw(KinectGesture gesture) {

        //}



        //Open a seperate port/thread for retriving stram data (should be moved to connection file if implemented)
        //Down prioritized till resizeing code is in place.
        public void MakeDataConnection()
        {
        }


        //This functon checks if resizeing is necesary. It resizes to 50% if it holds true and clamps the image.
        // set image.souce = ResizeImage( bitmap_image);
        public static TransformedBitmap ResizeImage(BitmapImage image)
        {
            double _H = image.PixelHeight;
            double _W = image.PixelWidth;
            if (_H > 200 || _W > 200) // makes no sense to resize if lower than 200 x 200
            { //need to resize
                //ugly assumation that we can resize 50% no matter what
                _H = Math.Floor( _H / 2d);
                _W = Math.Floor( _W / 2d);
                if (_W < 1) {_W = 1;}          if(_H < 1) { _H = 1; } //make sure nothing is 0 or negative
                if (_W > 960) { _W = 960; }    if (_H > 960) { _H = 960; } // make sure nothing is above 960 (half of 1920)
            }

            return ResizeImage(image, _W, _H, image.PixelWidth, image.PixelHeight);
         
        }

        // supposed to be called though ResizeImage(image);
        private static TransformedBitmap ResizeImage(BitmapImage image, double width, double height, double owidth, double oheight)
        {
            var bitmap = new TransformedBitmap(image, new ScaleTransform( width / owidth,  height / oheight));

            return bitmap;
        }

    }


}
