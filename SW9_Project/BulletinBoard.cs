using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace SW9_Project {
    //TODO: initialize label control - Set Zindex, - Recycle
    public class ScreenElement
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
            //TODO: get canvas size automaticly
            //canvas width 1113.6
            //canvas height 574.4
            double _H = image.PixelHeight;
            double _W = image.PixelWidth;
            bool wider = Math.Max(_W, _H) == _W;
            double ratio = _W / _H;
            if (_H > 200 || _W > 200) // makes no sense to resize if lower than 200 x 200
            { //need to resize
                //ugly assumation that we can resize 50% no matter what
                _H = Math.Floor(_H / 2d);
                _W = Math.Floor(_W / 2d);
                if (_W > 574) { _W = 574; }
                if (_H > 574) { _H = 574; } // make sure nothing is above 960 (half of 1920)
                //ensure ratio is kept in case the image was oversized
                if (ratio != (_W/_H))
                {
                    if (wider)
                    {
                        _H = Math.Floor(_W / ratio);
                    }
                    else
                    {
                        _W = Math.Floor(_H / ratio);
                    }
                }
                if (_W < 1) { _W = 1; }
                if (_H < 1) { _H = 1; } //make sure nothing is 0 or negative

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
        public readonly Image img; // the image, the image is based on the imgID - imgID is not stored.
        public readonly Label lbl; //label control, place right under document image
        //Constructors
        public ScreenElement( Image _image ) //image
        {
            type = Type.image;
            label = "";
            img = _image;
            img.Source = ResizeImage((BitmapImage)img.Source); //notice that source is now TransformedBitmap whether transformed or not.


        }

        public ScreenElement(string _label) //document
        {
            type = Type.document;
            if (_label == "" || _label == null)
            {
                Random r = new Random();
                int num = r.Next(GlobalVars.docStrings.Count);
                label = GlobalVars.docStrings[num];
            }
            else
            {
                label = _label;
            }

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

    public class ElementContainer
    {
        private List<ScreenElement> ElementList;
        private int pos; // Queue, max 99 - where you are about to add a element
        private Canvas canvasRef;
        public ElementContainer(Canvas c)
        {
            ElementList = new List<ScreenElement>();
            pos = 0;
            canvasRef = c;
        }
        //it is assumed that the image being added here is using a transformedbitmap as soruce
        // and that the source has been verified though the resize methods.
        public void AddElement(ScreenElement element, System.Windows.Point p)
        {
            //Allow for max 100
            if (pos == 99)
            {
                pos = 0;
                //Recycle
                foreach (var item in ElementList)
                {
                    Canvas.SetZIndex(item.img, Canvas.GetZIndex(item.img) - 100);
                    if (item.lbl != null)
                    {
                       Canvas.SetZIndex(item.lbl, Canvas.GetZIndex(item.lbl) - 100);
                    }
                }
            }
            if (ElementList[pos] != null)
            {
                canvasRef.Children.Remove(ElementList[pos].img);
                //Clear the image in this position. - probably overkill
                //ElementList[pos].img.Source = null;
                ElementList[pos] = null;
            }
            ElementList[pos] = element;
            // Add to canvas with the center as close to P as possible, without breaking borders.
            canvasRef.Children.Add(element.img);
            //TODO: Add function to calculate the extra space which is needed.
            Point sp=
            element.type == ScreenElement.Type.document ? GetSafeCoordinate(element.img, p, 0, 20) : GetSafeCoordinate(element.img, p);
            GetSafeCoordinate(element.img, p);
            Canvas.SetLeft(element.img, sp.X);
            Canvas.SetBottom(element.img, sp.Y);
            
            //Element added, increment position.
            pos++;
        }

        public ScreenElement GetElement()
        {
            return ElementList[pos];
        }

        public ScreenElement GetElement(int i)
        {
            return ElementList[i];
        }

        //might be unsafe
        public void ClearList()
        {
            foreach (var item in ElementList)
            {
                canvasRef.Children.Remove(item.img);
            }
            ElementList.Clear();
          pos = 0;
        }

        public int GetPos()
        { return pos; }

        //return a Point for the Left and Buttom, that causes the center of the element to be as close to point p as possible
        // without breaking the borders of the canvas
        // - I would love to use UIElement instead of Image, but i'm not sure about what numbers Rendersize are based on.
        // extrax and extra y is used to add more space than the image itself. - this is for documents with a label
        public Point GetSafeCoordinate(Image img, Point p, int extraX = 0, int extraY = 0 )
        {
            double left = 0;
            double buttom = 0;
            double imgCX = (img.Width + extraX) / 2;
            double imgCY = (img.Height + extraY) / 2;

            //resolve left (x)
            if (p.X <= imgCX) //clamp left
            {
                left = Math.Ceiling ( extraX/2d);
            }
            else if (p.X >= (canvasRef.Width - imgCX)) //clamp right
            {
                left = (canvasRef.Width - Math.Ceiling(img.Width + (extraX / 2d)));
            }
            else //no need for clamping
            {
                left = p.X - imgCX;
            }

            //resolve buttom (y)
            if (p.Y <= imgCY) //clamp top
            {
                buttom = Math.Ceiling(img.Height + (extraY / 2d));
            }
            else if (p.Y >= (canvasRef.Height - imgCY)) //clamp buttom
            {
                buttom = (canvasRef.Height - Math.Ceiling(extraY / 2d));
            }
            else // no need for clamping
            {
                buttom = p.Y - imgCY;
            }

            return new Point(left, buttom);
        }

    }

    public class BulletinBoard : CanvasWindow {

        static BulletinBoard instance;
        public ElementContainer elementContainer;
        /// <summary>
        /// I skal hold styr på background, om det skal være et billede eller farve skal ordnes ellers forsvinder griddet ikke
        /// At kalde base med false fortæller canvas window at det er en bulletin board der arbejder, den sætter variable targetPractice = false
        /// så bare bruge det hvis i skal ændre nogen ting i canvaswindow.cs
        /// </summary>
        private BulletinBoard() : base(false) {
            canvas.Background = Brushes.Gray;
            elementContainer = new ElementContainer(canvas); //used for storing images/documents on the screen.
            
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
        //postponed till second FT
        public void MakeDataConnection()
        {
        }


    }


}
