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
using System.Windows.Media.Effects;

namespace SW9_Project {

    /// <summary>
    /// Interaction logic for CanvasWindow.xaml
    /// </summary>
    public partial class CanvasWindow : Window, IDrawingBoard {

        UIElement pointerFigure;

        Dictionary<string, SoundPlayer> sounds = new Dictionary<string, SoundPlayer>();

        KinectManager kinectManager;

        bool accuracyTest = true; 

        Cell[,] grid;
        Cell target, extraTarget;
        Target nextTarget;
        GridSize currentSize;

        DataSource source;

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

        public CanvasWindow(bool accuracyTest = true) {
            
            this.accuracyTest = accuracyTest;

            targetColor = accuracyTest ? Brushes.White : Brushes.DarkGray;

            source = accuracyTest? DataSource.Accuracy : DataSource.Target;

            sounds.Add("hit", new SoundPlayer("resources/hit.wav"));
            sounds.Add("miss", new SoundPlayer("resources/miss.wav"));

            shapes = new List<String>();
            shapes.Add("circle");
            shapes.Add("square");
            GestureParser.Initialize(this);
            currentSize = GridSize.Small;
            window = this;
            InitializeComponent();
            kinectManager = new KinectManager(this);
            VideoWindow.SetCanvasWindow(this);

            
        }

        List<Line> cross = new List<Line>();
        private void ClearCross() {
            foreach(var c in cross) {
                canvas.Children.Remove(c);
            }
        }
        private List<Line> DrawCross(Cell cell) {

            double x = cell.X * squareWidth + (squareWidth * 0.5);
            double y = canvas.ActualHeight - (cell.Y * squareHeight + (squareHeight * 0.5));

            
            Line horLine = new Line(), verLine = new Line();

            horLine.X1 = x - 12; horLine.Y1 = y;
            horLine.X2 = x + 12; horLine.Y2 = y;

            verLine.X1 = x; verLine.Y1 = y + 12;
            verLine.X2 = x; verLine.Y2 = y - 12;

            verLine.StrokeThickness = 2;
            horLine.StrokeThickness = 2;
            verLine.Stroke = Brushes.White;
            horLine.Stroke = Brushes.White;

            cross.Clear();
            cross.Add(horLine); cross.Add(verLine);

            canvas.Children.Add(horLine);
            Canvas.SetZIndex(horLine, 600);

            canvas.Children.Add(verLine);
            Canvas.SetZIndex(verLine, 601);

            return cross;
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
                CreateGrid(lgWidth,lgHeight);
            } else {
                CreateGrid(sgWidth, sgHeight);
            }
            currentSize = size;
        }
        
        bool runningGesture = false;
        public void CurrentGestureDone() {
            foreach(var cell in grid) {
                cell.GridCell.Fill = Brushes.White;
            }
            runningGesture = false;
            this.Background = Brushes.Green;
        }

        public void StartNewGesture() {
            progressLabel.Content = $"Progress: {++gestureCount}/8";
            this.Background = Brushes.DarkGoldenrod;
            runningGesture = true;
            runningTest = true;
            UnlockPointer();
            GestureParser.ClearGestures();
        }
        

        public void PracticeDone() {
            this.Background = Brushes.Black;
        }

        internal static void SetPosition(double cx, double cy)
        {
            GyroPositionX = cx;
            GyroPositionY = cy;
        }

        private void CreateGrid(int width, int height) {
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
                    grid[i, j] = new Cell(ShapeFactory.CreateGridCell(squareWidth, squareHeight));
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

        int sCount = 0; string lastShape = "";
        private string GetNextShape() {

            string shape = shapes[randomizer.Next(shapes.Count)];

            if(shape != lastShape) {
                sCount = 0;
            }
            else if(++sCount > 2) {
                shape = shape == "square" ? "circle" : "square";
                sCount = 0;                
            }
            

            return shape;

        }
        JumpLength currentLength = JumpLength.NA;
        bool runningTest = false;
        public void DrawNextTargets() {
            if (runningTest && runningGesture) {
                if(target == null) {
                    ClearCross();
                    double size = squareWidth > squareHeight ? squareHeight : squareWidth;

                    string shape = GetNextShape();

                    if (currentSize != nextTarget.Size) {
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
                        if (accuracyTest) { DrawCross(extraTarget); }
                    }
                    currentLength = nextTarget.Length;
                    target = grid[nextTarget.X, nextTarget.Y];
                    target.GridCell.Fill = targetColor;
                    PushShape(shape, target);

                    target.Shape.Fill = Brushes.Black;
                    if (accuracyTest) { DrawCross(target); }

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
                currentCell.Fill = Brushes.White;
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

        bool savingToDB = false;
        public void PointAt(double xFromMid, double yFromMid) {

            if (pointerFigure == null) {
                pointerFigure = ShapeFactory.CreatePointer(accuracyTest);
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
            if(AttemptRepository.SaveStatus == DatabaseSaveStatus.Saving && !savingToDB) {
                savingToDB = true;
                ShowStatusMessage("Saving to database...");
            }
            if(savingToDB && AttemptRepository.SaveStatus != DatabaseSaveStatus.Saving) {
                savingToDB = false;
                string status = AttemptRepository.SaveStatus == DatabaseSaveStatus.Failed ? "Failed!" : "Success!";
                ShowStatusMessage(status);
                Background = AttemptRepository.SaveStatus == DatabaseSaveStatus.Failed ? Brushes.Red : Brushes.Blue;
            }

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
            if(!accuracyTest) ColorCell(pointer);
            KinectGesture gesture = GestureParser.AwaitingGesture;
            if (runningTest && runningGesture) {
                if (gesture != null) {
                    UnlockPointer();
                    GestureParser.Pause(true);
                    Cell currCell = GetCell(pointer);
                    bool hit = currCell == target;
                    bool correctShape = true;
                    
                    string shape = target.Shape is Ellipse ? "circle" : "square";
                    GestureDirection direction = GestureParser.GetDirectionContext();
                    GestureType type = GestureParser.GetTypeContext();
                    if (direction == GestureDirection.Push) {
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
            targetColor = accuracyTest ? Brushes.White : Brushes.DarkGray;
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
            Logger.Intialize(sgHeight, sgWidth, lgHeight, lgWidth, canvas.ActualHeight, canvas.ActualWidth, source);
        }
        

        public Point GetPoint(double xFromMid, double yFromMid)
        {
            double x = Scale(canvas.ActualWidth, .25f, xFromMid);
            double y = Scale(canvas.ActualHeight, .20f, yFromMid);
            Point p = new Point(x, y);

            return p;
        }

        public void EndTest() {
            currentTest = null;
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
        Queue<GestureType> types = new Queue<GestureType>(DataGenerator.AllTechniques);
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {

            if (e.Key == System.Windows.Input.Key.Space) {
                Background = Brushes.Black;/*
                if (connection == null || !connection.Connected) {
                    connectedLabel.BeginAnimation(Canvas.OpacityProperty, CreateAnimation(5, 1, 0));
                    return;
                }*/
                if (currentTest == null) {
                    currentTest = new TestSuite(this, source);
                    kinectManager.Recalibrate();
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

            else if(e.Key == System.Windows.Input.Key.I) {
                PointAt(0, 0);
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

            else if(e.Key == System.Windows.Input.Key.Enter) {
                kinectManager.Recalibrate();
                ShowStatusMessage("Recalibrating...");
            }

            else if (e.Key == System.Windows.Input.Key.U) {
                VideoWindow.PlayVideo(GestureDirection.Pull, types.Dequeue());
            }
            
            else if (e.Key == System.Windows.Input.Key.A) {
                ShowStatusMessage("Push");
                GestureParser.SetDirectionContext(GestureDirection.Push);
            } else if (e.Key == System.Windows.Input.Key.S) {
                ShowStatusMessage("Pull");
                GestureParser.SetDirectionContext(GestureDirection.Pull);
            }
        }

        private void ShowStatusMessage(string message) {
            gestureTypeLabel.Content = message;
            gestureTypeLabel.BeginAnimation(Canvas.OpacityProperty, CreateAnimation(5, 1, 0));

        }

        private void StartDebugTest(GestureType type) {
            if (connection == null || !connection.Connected) {
                connectedLabel.BeginAnimation(Canvas.OpacityProperty, CreateAnimation(5, 1, 0));
                return;
            }
            Logger.CurrentLogger.DebugMode = true;

            ShowStatusMessage(type.ToString());
            currentTest = new TestSuite(this, source);
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
        private int gestureCount = 0;

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
