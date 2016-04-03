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

    class ScreenElement
    {
        //Helpers
        public enum Type
        {
            document, // 0
            image // 1
        };

        //This functon checks if resizeing is necesary. It resizes to 50% if it holds true and clamps the image.
        // set image.souce = ResizeImage( bitmap_image);
        public static TransformedBitmap ResizeImage(BitmapImage image)
        {
            double _H = image.PixelHeight;
            double _W = image.PixelWidth;
            if (_H > 200 || _W > 200) // makes no sense to resize if lower than 200 x 200
            { //need to resize
                //ugly assumation that we can resize 50% no matter what
                _H = Math.Floor(_H / 2d);
                _W = Math.Floor(_W / 2d);
                if (_W < 1) { _W = 1; }
                if (_H < 1) { _H = 1; } //make sure nothing is 0 or negative
                if (_W > 960) { _W = 960; }
                if (_H > 960) { _H = 960; } // make sure nothing is above 960 (half of 1920)
            }

            return ResizeImage(image, _W, _H, image.PixelWidth, image.PixelHeight);

        }

        // supposed to be called though ResizeImage(image) - left available for manual manipulation
        public static TransformedBitmap ResizeImage(BitmapImage image, double width, double height, double owidth, double oheight)
        {
            var bitmap = new TransformedBitmap(image, new ScaleTransform(width / owidth, height / oheight));
            return bitmap;
        }

        //Members

        public readonly Type type;  // used to identify if it is image or document - not sure if needed.
        public readonly string label; //used with document
        public readonly System.Windows.Controls.Image img; // the image, the image is based on the imgID
                                                            // thus imgID is not part of the container
        
        //Constructors
        public ScreenElement( System.Windows.Controls.Image _image ) //image
        {
            type = Type.image;
            label = "";
            img = _image;
            img.Source = ResizeImage((BitmapImage)img.Source); //notice that source is now TransformedBitmap whether transformed or not.


        }

        public ScreenElement(string _label) //document
        {
            type = Type.document;
            label = _label; //change to random selected string ?
            img = new System.Windows.Controls.Image();
            //TODO: change to default document image used for posting
            img.Source = new BitmapImage(new Uri("resources/ImageShape.png", UriKind.RelativeOrAbsolute));
            // transform to a transformedBitmap object ?

        }
        //in case someone wants to define everything, ex: image with a label perhaps.
        //Perform manual manipulation of image before adding.
        public ScreenElement(Type _type, string _label , System.Windows.Controls.Image _image) //document
        {
            type = _type;
            label = _label; 
            img = _image;
        }

        //TODO: Run a test to determine whether a desctuctor is need to prevent memory leak
        // Test should be to assest the memory at start.
        // add 10000 elements in reursion (element container handles what is neccesary)
        // assest memory and see if there are a significant memory leak.
        //more info: #5 in https://blogs.msdn.microsoft.com/jgoldb/2008/02/04/finding-memory-leaks-in-wpf-based-applications/

    }

    class ElementContainer
    {
        private List<ScreenElement> ElementList;
        private int pos; // Queue, max 99
        public ElementContainer()
        {
            ElementList = new List<ScreenElement>();
            pos = 0;
        }
        //it is assumed that the image being added here is using a transformedbitmap as soruce
        // and that the source has been verified though those methods.
        public void AddElement(ScreenElement i)
        {
            //Allow for max 100
            if (pos >= 99)
            {
                pos = 0;
            }
            if (ElementList[pos] != null)
            {
                //Clear the image in this position. - probably overkill
                //ElementList[pos].img.Source = null;
                ElementList[pos] = null;
                // TODO: remove UI element from canvas.children.
            }
            ElementList[pos] = i;
        }
        //might be unsafe
        public void ClearList()
        { ElementList.Clear(); }
    }

    public class BulletinBoard : CanvasWindow {

        static BulletinBoard instance;
        ElementContainer elementContainer;
        /// <summary>
        /// I skal hold styr på background, om det skal være et billede eller farve skal ordnes ellers forsvinder griddet ikke
        /// At kalde base med false fortæller canvas window at det er en bulletin board der arbejder, den sætter variable targetPractice = false
        /// så bare bruge det hvis i skal ændre nogen ting i canvaswindow.cs
        /// </summary>
        private BulletinBoard() : base(false) {
            canvas.Background = Brushes.Gray;
            elementContainer = new ElementContainer(); //used for storing images/documents on the screen.

            //TODO: target image MUST float ontop of exsiting images.
            
        }

        public static BulletinBoard Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BulletinBoard();
                }
                return instance;
            }
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


    }


}
