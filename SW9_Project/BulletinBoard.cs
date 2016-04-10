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
using System.Windows.Threading;

namespace SW9_Project {
    public class ScreenElement
    {
        //Helpers
        public enum Type
        {
            document,
            image,
            other
        };

        //TODO: fix this converter for future usage.
        // OUT OF MEMORY ERROR
        public static BitmapSource ConvertBitmapTo96DPI(BitmapImage bitmapImage)
        {
            double dpi = 96;
            int width = bitmapImage.PixelWidth;
            int height = bitmapImage.PixelHeight;

            int stride = width * bitmapImage.Format.BitsPerPixel;
            byte[] pixelData = new byte[stride * height];
            bitmapImage.CopyPixels(pixelData, stride, 0);

            return BitmapSource.Create(width, height, dpi, dpi, bitmapImage.Format, null, pixelData, stride);
        }

        //This functon checks if resizeing is necesary. It resizes to 50% if it holds true and clamps the image.
        // set image.souce = ResizeImage( bitmap_image);
        //System only support 96 DPI images
        public static TransformedBitmap ResizeImage(BitmapImage image)
        {
            
            double maxSize = Math.Min(GlobalVars.canvasWidth, GlobalVars.canvasHeight); //max size allowed is based on the smalest size.
            maxSize = Math.Floor(maxSize / 3d);
            //canvas width 1852,8
            //canvas height 969,6
            double _H = image.PixelHeight;
            double _W = image.PixelWidth;
            bool wider = Math.Max(_W, _H) == _W;
            double ratio = _W / _H;
            if (_H > 200 || _W > 200) // makes no sense to resize if lower than 200 x 200
            { //need to resize
                //ugly assumation that we can resize 50% no matter what
                _H = Math.Floor(_H / 2d);
                _W = Math.Floor(_W / 2d);
                if (_W > maxSize) { _W = maxSize; }
                if (_H > maxSize) { _H = maxSize; } // make sure nothing is above max allowed size
                //ensure ratio is kept in case the image was oversized
                if (ratio != (_W/_H))
                {
                    if (wider)
                    {
                        _H = Math.Floor(_W / ratio);
                    }
                    else
                    {
                        _W = Math.Floor(_H * ratio);
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
        public readonly Image img; // the image, the image is based on the imgID - imgID is not stored.
        public readonly Label lbl; //label control, place right under document image
                                   
        //Constructors

        public ScreenElement(int imgid) //image
        {
            BitmapImage _bitmap;
            img = new Image();
            if ( GlobalVars.imgDict.TryGetValue(imgid,out _bitmap ) ) //fetch image if imgid matches.
            {}
            else{ GlobalVars.imgDict.TryGetValue(0, out _bitmap);  } //otherwise fetch the default image

            img.Source = ResizeImage(_bitmap); //notice that source is now TransformedBitmap whether transformed or not.
            type = Type.image;
        }

        public ScreenElement( Image _image ) //image
        {
            type = Type.image;
            img = _image;
            img.Source = ResizeImage((BitmapImage)img.Source); //notice that source is now TransformedBitmap whether transformed or not.

        }

        public ScreenElement(string _label) //document
        {
            type = Type.document;
            lbl = new Label();
            if (_label == "" || _label == null)
            {
                Random r = new Random();
                int num = r.Next(GlobalVars.docStrings.Count-1);
                lbl.Content = GlobalVars.docStrings[num];
            }
            else
            {
                lbl.Content = _label;
            }
            BitmapImage _bitmap;
            GlobalVars.imgDict.TryGetValue(0, out _bitmap);
            img = new Image();
            img.Source = _bitmap;
            // transform to a transformedBitmap object ?
        }
        //in case someone wants to define everything, ex: image with a label perhaps.
        //Perform manual manipulation of image before adding.
        public ScreenElement(Image _image, Label _lbl, Type _type = Type.other) //document
        {
            type = _type;
            img = _image;
            lbl = _lbl;
            
        }
    }



    public class ElementContainer
    {
        private List<ScreenElement> ElementList;
        private int pos; // Queue, max 100 - where you are about to add a element
        private Canvas canvasRef;

        //constructor
        public ElementContainer(Canvas c)
        {
            ElementList = new List<ScreenElement>();
            pos = 0;
            canvasRef = c;
        }

        //Methods

        //it is assumed that the image being added here is using a transformedbitmap as soruce
        // and that the source has been verified though the resize methods.
        //it was originally assumed there would allways be an image - since the constructor allows for no image, I have tried to check for it.
        //it is however not validated to be secure without an image.
        public void AddElement(ScreenElement element, System.Windows.Point p)
        {
            if (element.img == null && element.lbl == null)
            {
                return; //at least one UIelement must be available.
            }
            //Allow for max 100
            if (pos == 100)
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
            if (ElementList.Count > pos && ElementList[pos] != null)
            {
                if (ElementList[pos].lbl != null)
                {
                    canvasRef.Children.Remove(ElementList[pos].lbl);
                }
                if (ElementList[pos].img != null) //Its expected the image is available, but constructors allows for image not to be.
                {
                    canvasRef.Children.Remove(ElementList[pos].img);
                }
                

                //Clear the image in this position. - probably overkill
                //ElementList[pos].img.Source = null;
                ElementList[pos] = null;
            }
            if (ElementList.Count > pos)
            {
                ElementList[pos] = element;
            }
            else
            {
                ElementList.Add(element);
            }
            
            // Add to canvas with the center as close to P as possible, without breaking borders.
            if (element.img != null)
            {
                canvasRef.Children.Add(element.img);
                
                Size lblSize = new Size(0, 0);
                if (element.lbl != null)
                {
                    lblSize = FitLabelWidth(element.lbl, element.img.Source.Width);
                }
                //extra width is set to 10, as label is fit to be inside img width.
                Point sp = GetSafeCoordinate(element.img, p, 10, lblSize.Height);
                Canvas.SetLeft(element.img, sp.X);
                Canvas.SetBottom(element.img, sp.Y);
                Canvas.SetZIndex(element.img, 501 + pos);
                if (element.lbl != null)
                {
                    canvasRef.Children.Add(element.lbl);
                    Canvas.SetBottom(element.lbl, sp.Y - lblSize.Height);
                    Canvas.SetLeft(element.lbl, (sp.X + element.img.Source.Width/2) - lblSize.Width/2 - 5);
                    Canvas.SetZIndex(element.lbl, 501 + pos);
                }
            }
            else
            {
                
                Size lblSize = FitLabelWidth(element.lbl, 200);

                //Notice that this doesn't have a safe coordinate
                canvasRef.Children.Add(element.lbl);
                Canvas.SetBottom(element.lbl, p.Y - lblSize.Height/2 );
                Canvas.SetLeft(element.lbl, p.X - lblSize.Width / 2);
                Canvas.SetZIndex(element.lbl, 501 + pos);
            }

            //Element added, increment position.
            pos++;
        }

        private Size FitLabelWidth(Label lbl,double maxWidth )
        {

            Double fontSize = 21;
            Size textSize;
            String text = lbl.Content.ToString();
            do
            {
                fontSize--;

                Typeface myTypeface = new Typeface("Segoe UI");
                FormattedText ft = new FormattedText(text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    myTypeface, fontSize, Brushes.Black);

                textSize = new Size(ft.Width, ft.Height);

            } while (fontSize > 1 && textSize.Width > maxWidth || textSize.Height >30 );

            lbl.FontSize = fontSize;
            lbl.Background = Brushes.LightGray;
            Size textBackground = new Size(textSize.Width, textSize.Height + 10); //10 above and below.

            return textBackground;
        }

        public ScreenElement GetElement()
        {
            return ElementList[pos];
        }

        public ScreenElement GetElement(int i)
        {
            return ElementList[i];
        }


        public void ClearList()
        {
            foreach (var item in ElementList)
            {
                if (item.img != null)
                {
                    canvasRef.Children.Remove(item.img);
                }
                if (item.lbl != null)
                {
                    canvasRef.Children.Remove(item.lbl);
                }

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
        public Point GetSafeCoordinate(Image img, Point p, double extraX = 0, double extraY = 0 )
        {
            double left = 0;
            double buttom = 0;
            double imgCX = (img.Source.Width + extraX) / 2;
            double imgCY = (img.Source.Height + extraY) / 2;

            //resolve left (x)
            if (p.X <= imgCX) //clamp left
            {
                left = Math.Ceiling ( extraX/2d);
            }
            else if (p.X >= (GlobalVars.canvasWidth - imgCX)) //clamp right
            {
                left = (GlobalVars.canvasWidth - Math.Ceiling(img.Source.Width + (extraX / 2d)));
            }
            else //no need for clamping
            {
                left = p.X - imgCX;
            }

            //resolve buttom (y)
            if (p.Y <= imgCY) //clamp buttom
            {
                buttom = Math.Ceiling((extraY));
            }
            else if (p.Y >= (GlobalVars.canvasHeight - imgCY)) //clamp top
            {
                buttom = (GlobalVars.canvasHeight - Math.Ceiling(img.Source.Height));
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
            progressLabel.VerticalAlignment = VerticalAlignment.Bottom;
            progressLabel.HorizontalAlignment = HorizontalAlignment.Center;
            Thickness margin = new Thickness(0, 0, 0, 10);
            progressLabel.Margin = margin;

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


        //TODO: Run a test to determine whether a desctuctor is need to prevent memory leak
        // Test should be to assest the memory at start.
        // add 10000 elements in reursion (element container handles what is neccesary)
        // assest memory and see if there are a significant memory leak.
        //more info: #5 in https://blogs.msdn.microsoft.com/jgoldb/2008/02/04/finding-memory-leaks-in-wpf-based-applications/

        public void PerformanceTest()
        {
            List<int> IL = Enumerable.ToList(GlobalVars.imgDict.Keys);
            int size = IL.Count;
            System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
            Point p;
            Random rnd = new Random();
            int initialSize = 0, LoadedSize = 0, memsize = 0, memsize2 = 0, memsize3 = 0; // memsize in KB
            System.Diagnostics.PerformanceCounter PC = new System.Diagnostics.PerformanceCounter();
            PC.CategoryName = "Process";
            PC.CounterName = "Working Set - Private";
            PC.InstanceName = proc.ProcessName;
            initialSize = Convert.ToInt32(PC.NextValue()) / (int)(1024);
            ThanksLabel.Content = "TEST 1";
            ThanksLabel.Visibility = Visibility.Visible;
            foreach (var item in GlobalVars.imgDict)
            {
                p = new Point((double)rnd.Next((int)GlobalVars.canvasWidth), (double)rnd.Next((int)GlobalVars.canvasHeight));
                this.PointAt(p.X, p.Y);
                ScreenElement se = new ScreenElement(item.Key);
                elementContainer.AddElement(se, p);
                ProcessUITasks();
            }

            this.Activate();

            LoadedSize = Convert.ToInt32(PC.NextValue()) / (int)(1024);
            elementContainer.ClearList();
            ProcessUITasks();
            this.Activate();
            Console.WriteLine("initial size=" + initialSize);
            Console.WriteLine("Loaded size=" + LoadedSize);
            ThanksLabel.Content = "TEST 2";
            for (int i = 1; i < 11; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    p = new Point((double)rnd.Next((int)GlobalVars.canvasWidth), (double)rnd.Next((int)GlobalVars.canvasHeight));
                    this.PointAt(p.X, p.Y);
                    ScreenElement se = new ScreenElement(IL[rnd.Next(size)]);
                    elementContainer.AddElement(se, p);
                    ProcessUITasks();
                }
                ThanksLabel.Content = $"TEST 2:{i}";
                memsize = Convert.ToInt32(PC.NextValue()) / (int)(1024);
                Console.WriteLine("size at " + i + "000 =" + memsize);
            }
            this.Activate();
            memsize2 = Convert.ToInt32(PC.NextValue()) / (int)(1024);
            elementContainer.ClearList();
            ProcessUITasks();
            this.Activate();
            memsize3 = Convert.ToInt32(PC.NextValue()) / (int)(1024);

            Console.WriteLine("After 10000=" + memsize2);
            Console.WriteLine("After Cleared=" + memsize3);

            ProcessUITasks();

            int frames = 0;
            int maxframes = 30 * 10; //10 seconds

            this._StartVideoRecording(9999999);
            while (frames < maxframes)
            {
                ThanksLabel.Content = $"KinectRecording: {frames} / {maxframes}";
                ProcessUITasks();
                frames++;
            }
            memsize2 = Convert.ToInt32(PC.NextValue()) / (int)(1024);
            _StopVideoRecording();
            ProcessUITasks();
            frames = 0;
            while (frames < 90)
            {
                ProcessUITasks();
                frames++;
            }
            ThanksLabel.Content = "Thank you for testing!";
            ThanksLabel.Visibility = Visibility.Collapsed;
            memsize3 = Convert.ToInt32(PC.NextValue()) / (int)(1024);
            Console.WriteLine("After recording = " + memsize2);
            Console.WriteLine("After closed = " + memsize3);

            PC.Close();
            PC.Dispose();
        }

        private static void ProcessUITasks()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (object parameter) {
                frame.Continue = false;
                return null;
            }), null);
            Dispatcher.PushFrame(frame);
        }

    }


}
