using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using SW9_Project.Logging;
using DataSetGenerator;
using System.Windows.Media.Animation;
using System.Media;

using Point = System.Windows.Point;

namespace SW9_Project {

    /// <summary>
    /// Interaction logic for CanvasWindow.xaml
    /// </summary>
    public partial class CanvasWindow : Window, IDrawingBoard {

        UIElement pointerFigure;

        Dictionary<string, SoundPlayer> sounds = new Dictionary<string, SoundPlayer>();

        KinectManager kinectManager;

        bool targetPractice = true;

        Cell[,] grid;
        Cell target, extraTarget;
        Target nextTarget;
        GridSize currentSize;

        int gridHeight, gridWidth;
        public static int sgHeight = 10, sgWidth = 20, lgHeight = sgHeight/2, lgWidth = sgWidth/2;
        double squareHeight = 0, squareWidth = 0;

        static CanvasWindow window;
        static Connection connection;
        TestSuite currentTest;
        List<String> shapes;
        Brush targetColor = Brushes.DarkGray;
        Point lastGyroPoint { get; set; }
        
        double xPoint = 0;
        double yPoint = 0;

        public CanvasWindow(bool targetPractice = true) {
            
            this.targetPractice = targetPractice;

            sounds.Add("hit", new SoundPlayer("resources/hit.wav"));
            sounds.Add("miss", new SoundPlayer("resources/miss.wav"));

            shapes = new List<String>();
            shapes.Add("circle");
            shapes.Add("square");
            GestureParser.Initialize(this);
            currentSize = targetPractice ? GridSize.Small : GridSize.Large;
            window = this;
            InitializeComponent();
            kinectManager = new KinectManager(this);
            VideoWindow.SetCanvasWindow(this);
        }

        public static void SetConnection(Connection _connection) {
            connection = _connection;
            GestureParser.SetConnection(connection);
        }

        public void Clear() {
            this.Background = Brushes.Black;
            target = null;
            extraTarget = null;
            nextTarget = null;
            CreateGrid(currentSize);
        }

        public void CreateGrid(GridSize size) {
            if(size == GridSize.Large) {
                CreateGrid(lgWidth,lgHeight, targetPractice);
            } else {
                CreateGrid(sgWidth, sgHeight, targetPractice);
            }
            currentSize = size;
        }
        
        bool runningGesture = false;
        public void CurrentGestureDone() {
            GestureParser.SetTypeContext(GestureType.Swipe);
            foreach(var cell in grid) {
                cell.GridCell.Fill = Brushes.White;
            }
            runningGesture = false;
            this.Background = Brushes.Green;
        }

        public void StartNewGesture() {
            this.Background = Brushes.DarkGoldenrod;
            runningGesture = true;
            runningTest = true;
        }

        public void PracticeDone() {
            this.Background = Brushes.Black;
        }

        internal static void SetPosition(double cx, double cy)
        {
            GyroPositionX = cx;
            GyroPositionY = cy;
        }

        private void CreateGrid(int width, int height, bool includeBorders) {
            if(grid != null) {
                canvas.Children.Clear();
            }
            if(pointerFigure != null) {
               canvas.Children.Add(pointerFigure);
            }
            gridHeight = height;
            gridWidth = width;
            squareHeight = canvas.ActualHeight / height;
            squareWidth = canvas.ActualWidth / width;

            grid = new Cell[width, height];

            for(int i = 0; i < width; i++) {
                for(int j = 0; j < height; j++) {
                    grid[i, j] = new Cell(ShapeFactory.CreateGridCell(squareWidth, squareHeight, includeBorders));
                    grid[i, j].X = i; grid[i, j].Y = j;
                    canvas.Children.Add(grid[i, j].GridCell);
                    Canvas.SetBottom(grid[i, j].GridCell, j * squareHeight);
                    Canvas.SetLeft(grid[i, j].GridCell, i * squareWidth);
                    Canvas.SetZIndex(grid[i, j].GridCell, 0);
                }
            }
        }

        private Random randomizer = new Random();

        public void CreateTarget(Target target) {
            nextTarget = target;
            
        }

        bool runningTest = false;
        JumpLength currentLength = JumpLength.NA;
        public void DrawNextTargets() {
            if (runningTest && runningGesture) {
                if(target == null) {
                    double size = squareWidth > squareHeight ? squareHeight : squareWidth;
                    string shape = shapes[randomizer.Next(shapes.Count)];
                    if(currentSize != nextTarget.Size) {
                        Logger.CurrentLogger.ChangeSize(nextTarget.Size);
                    }
                    CreateGrid(nextTarget.Size);

                    if (GestureParser.GetDirectionContext() == GestureDirection.Pull) {
                        connection?.SetNextShape(shape);
                        string extraShape = shape == "circle" ? "square" : "circle";
                        int t = nextTarget.Size == GridSize.Large ? 1 : 2;
                        List<int> xPossibilities = new List<int>();
                        List<int> yPossibilities = new List<int>();

                        for(int i = 0; i < gridWidth; i++) {
                            if(i < nextTarget.X - t || i > nextTarget.X + t) {
                                xPossibilities.Add(i);
                            }
                        }
                        for(int i = 0; i < gridHeight; i++) {
                            if(i < nextTarget.Y -t || i > nextTarget.Y + t) {
                                yPossibilities.Add(i);
                            }
                        }
                        int x = xPossibilities[randomizer.Next(xPossibilities.Count)], y = yPossibilities[randomizer.Next(yPossibilities.Count)];
                        extraTarget = grid[x,y];
                        extraTarget.GridCell.Fill = targetColor;
                        PushShape(extraShape, extraTarget);
                        extraTarget.Shape.Fill = Brushes.Black;
                    }
                    currentLength = nextTarget.Length;
                    target = grid[nextTarget.X, nextTarget.Y];
                    target.GridCell.Fill = targetColor;
                    PushShape(shape, target);

                    target.Shape.Fill = Brushes.Black;

                }
            }
        }

        public Cell GetCell(Point p) {


            int x = (int)Math.Floor(p.X / squareWidth);
            int y = (int)Math.Floor(p.Y / squareHeight);

            if (x >= gridWidth) { x = gridWidth - 1; }
            if (y >= gridHeight) { y = gridHeight - 1; }

            return grid[x, y]; 

        }

        Rectangle currentCell;
        private void ColorCell(Point toColor) {
            
            if (currentCell != null) {
                currentCell.Fill = targetPractice ? Brushes.White : Brushes.Transparent;
            }
            currentCell = GetCell(toColor).GridCell;
            currentCell.Fill = Brushes.Yellow;
            //currentCell.Fill.Opacity = 0.5;
        }
        
        protected Point pointer = new Point();
        private bool lockedPointer = false;


        public void LockPointer() {
            if(pointerFigure is Shape) {
                ((Shape)pointerFigure).Fill = Brushes.Red;
            }
            lockedPointer = true;
        }

        public void UnlockPointer() {
            if (pointerFigure is Shape) {
                ((Shape)pointerFigure).Fill = Brushes.Blue;
            }
            lockedPointer = false;
        }

        public void ResetGyro()
        {
            GyroPositionX = 0;
            GyroPositionY = 0;
        }
        
        public void PointAt(double xFromMid, double yFromMid) {

            if (pointerFigure == null) {
                pointerFigure = ShapeFactory.CreatePointer();
                canvas.Children.Add(pointerFigure);
                Canvas.SetZIndex(pointerFigure, 10000);
                xPoint = xFromMid;
                yPoint = yFromMid;
            }
            if (target != null) {
                target.GridCell.Fill = targetColor;
            }
            if(extraTarget != null) {
                extraTarget.GridCell.Fill = targetColor;
            }

            DrawNextTargets();

            Point currentGyroPoint = new Point(GyroPositionX, -GyroPositionY);
            if (currentGyroPoint != lastGyroPoint)
            {
                lastGyroPoint = new Point(GyroPositionX, -GyroPositionY);
            }

            xPoint = xFromMid;
            yPoint = yFromMid;

            if (!lockedPointer) {
                pointer = GetPoint(xPoint, yPoint);
            }
            MoveShape(pointerFigure, pointer);
            ColorCell(pointer);
            KinectGesture gesture = GestureParser.AwaitingGesture;
            if (runningTest && runningGesture) {
                if (gesture != null) {
                    UnlockPointer();
                    GestureParser.Pause(true);
                    Cell currCell = GetCell(pointer);
                    bool hit = currCell == target;
                    bool correctShape = true;
                    string shape = target.Shape is Ellipse ? "circle" : "square";
                    if (GestureParser.GetDirectionContext() == GestureDirection.Push) {
                        correctShape = shape == gesture.Shape;
                    }
                    currentTest.TargetHit(hit, correctShape, target, pointer, currCell, currentLength);
                    if (hit && !correctShape) { hit = false; }
                    TargetHit(target, hit);
                }
            }
            ExtendedDraw(gesture);
        }

        public virtual void ExtendedDraw(KinectGesture gesture) {

        }

        public static Point GetCurrentPoint() {
            return window.pointer;
        }

        private Shape PullShape(Point p) {
            Cell cell = GetCell(p);
            Shape returnShape = cell.Shape;
            canvas.Children.Remove(returnShape);
            cell.Shape = null;
            return returnShape;
        }

        private void PushShape(string shape, Point p) {
            PushShape(shape, GetCell(p));
        }

        private void PushShape(string shape, Cell cell) {
            canvas.Children.Remove(cell?.Shape);
            double size = squareHeight < squareWidth ? squareHeight : squareWidth;
            Shape t = ShapeFactory.CreateShape(shape, size);
            double x = Canvas.GetLeft(cell.GridCell) + (cell.GridCell.Width / 2);
            double y = Canvas.GetBottom(cell.GridCell) + (cell.GridCell.Height / 2);
            Canvas.SetZIndex(t, 500);

            canvas.Children.Add(t);
            cell.Shape = t;

            Point f = new Point(x, y);
            MoveShape(cell.Shape, f);
            
        }
        private void TargetHit(Cell cell, bool hit) {
            connection?.SwitchShapes();
            if (hit) {
                sounds["hit"].Play();
                cell.Shape.Fill = Brushes.Green;
            }
            else {
                sounds["miss"].Play();
                cell.Shape.Fill = Brushes.Red;
            }
            DoubleAnimation da = new DoubleAnimation(0, TimeSpan.FromSeconds(1));
            da.Completed += (sender, e) => Da_Completed(sender, e, target);
            targetColor = Brushes.White;
            cell.Shape.BeginAnimation(Canvas.OpacityProperty, da);

            if(extraTarget != null) {
                canvas.Children.Remove(extraTarget.Shape);
                extraTarget.Shape = null;
            }

            
        }
        private void Da_Completed(object sender, EventArgs e, Cell cell) {
            GestureParser.Pause(false);
            if (target == null)
                return;
            Cell t = target;
            if(nextTarget == null) {
                runningTest = false;
            }
            target = null;
            t.GridCell.Fill = Brushes.White;
            targetColor = Brushes.DarkGray;
            canvas.Children.Remove(cell.Shape);
            cell.Shape = null;
        }

        private void MoveShape(UIElement shapeToMove, Point p) {
            double x = p.X - 24;// - (24 / 2);
            double y = p.Y - 24;// - (24 / 2);
            if (shapeToMove is Shape) {
                Shape t = shapeToMove as Shape;
                x = p.X - (t.Width / 2);
                y = p.Y - (t.Height / 2);
                if (Double.IsNaN(t.Width) || Double.IsNaN(t.Height)) {
                    double size = squareWidth < squareHeight ? squareWidth : squareHeight;
                    x = p.X - (size / 2);
                    y = p.Y - (size / 2);
                }
            }
            
            Canvas.SetLeft(shapeToMove, x);
            Canvas.SetBottom(shapeToMove, y);
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e) {
            if(canvas.Children.Count != 0) {
                canvas.Children.RemoveRange(0, canvas.Children.Count);
            }
            CreateGrid(currentSize);
            TestSuite.Intialize(sgHeight, sgWidth, lgHeight, lgWidth, canvas.ActualHeight, canvas.ActualWidth);
            Logger.Intialize(sgHeight, sgWidth, lgHeight, lgWidth, canvas.ActualHeight, canvas.ActualWidth);
        }
        

        public Point GetPoint(double xFromMid, double yFromMid)
        {
            double x = Scale(canvas.ActualWidth, .25f, xFromMid);
            double y = Scale(canvas.ActualHeight, .26f, yFromMid);
            Point p = new Point(x, y);

            return p;
        }

        public Point GetPoint(double xFromMid, double yFromMid, float scaleX, float scaleY)
        {
            double x = Scale(canvas.ActualWidth, scaleX, xFromMid);
            double y = Scale(canvas.ActualHeight, scaleY, yFromMid);
            Point p = new Point(x, y);

            return p;
        }

        public void EndTest() {
            runningTest = false;
        }

        private DoubleAnimation CreateAnimation(int seconds, double from, double to) {
            DoubleAnimation da = new DoubleAnimation();
            da.From = from;
            da.To = to;
            da.Duration = TimeSpan.FromSeconds(seconds);
            return da;
        }

        /// <summary>
        /// Register a key down event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {

            if (e.Key == System.Windows.Input.Key.Space) {
                if (connection == null || !connection.Connected) {
                    connectedLabel.BeginAnimation(Canvas.OpacityProperty, CreateAnimation(5, 1, 0));
                    return;
                }
                if (currentTest == null || currentTest.Done) {
                    if (currentTest?.Done == true)
                    {
                        Application.Current.Shutdown(); // app close
                    }
                    currentTest = new TestSuite(this);
                    testIDLabel.Content = "User ID: " + currentTest.UserID;
                    testIDLabel.BeginAnimation(Canvas.OpacityProperty, CreateAnimation(10, 1, 0));
                } else if (runningTest && !runningGesture) {
                    currentTest.ChangeGesture();
                } 
            } else if (e.Key == System.Windows.Input.Key.Up) {
                if (currentTest != null) { 
                    currentTest.StartTest(GestureDirection.Push);
                }
            } else if (e.Key == System.Windows.Input.Key.Down) {
                if (currentTest != null) {
                    currentTest.StartTest(GestureDirection.Pull);
                }
            } 
            
            else if (e.Key == System.Windows.Input.Key.Q) {
                StartDebugTest(GestureType.Swipe);
            } else if (e.Key == System.Windows.Input.Key.W) {
                StartDebugTest(GestureType.Throw);
            } else if (e.Key == System.Windows.Input.Key.E) {
                StartDebugTest(GestureType.Pinch);
            } else if (e.Key == System.Windows.Input.Key.R) {
                StartDebugTest(GestureType.Tilt);
            } 
            
            else if (e.Key == System.Windows.Input.Key.A) {
                gestureTypeLabel.Content = "Push";
                gestureTypeLabel.BeginAnimation(Canvas.OpacityProperty, CreateAnimation(5, 1, 0));
                GestureParser.SetDirectionContext(GestureDirection.Push);
            } else if (e.Key == System.Windows.Input.Key.S) {
                gestureTypeLabel.Content = "Pull";
                gestureTypeLabel.BeginAnimation(Canvas.OpacityProperty, CreateAnimation(5, 1, 0));
                GestureParser.SetDirectionContext(GestureDirection.Pull);
            }
        }

        private void StartDebugTest(GestureType type) {
            if (connection == null || !connection.Connected) {
                connectedLabel.BeginAnimation(Canvas.OpacityProperty, CreateAnimation(5, 1, 0));
                return;
            }
            Logger.CurrentLogger.DebugMode = true;
            gestureTypeLabel.Content = type.ToString();
            gestureTypeLabel.BeginAnimation(Canvas.OpacityProperty, CreateAnimation(5, 1, 0));
            currentTest = new TestSuite(this);
            currentTest.StartDebugTest(type);

        }

        private static double Scale(double maxPixel, float maxSkeleton, double position)
        {
            double value = ((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));
            if (value > maxPixel)
                return maxPixel;
            if (value < 0)
                return 0;
            return value;
        }

        private void canvas_Loaded(object sender, RoutedEventArgs e) {
            CreateGrid(currentSize);
        }

        private bool _inStateChange;

        public static double GyroPositionX { get; set; }
        public static double GyroPositionY { get; set; }

        protected override void OnStateChanged(EventArgs e) {
            if (WindowState == WindowState.Maximized && !_inStateChange) {
                _inStateChange = true;
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                ResizeMode = ResizeMode.NoResize;
                _inStateChange = false;
            }
            base.OnStateChanged(e);
        }
    }
}
