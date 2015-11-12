using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using SW9_Project.Logging;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace SW9_Project {

    public enum GridSize { Small, Large }
    /// <summary>
    /// Interaction logic for CanvasWindow.xaml
    /// </summary>
    public partial class CanvasWindow : Window, IDrawingBoard {

        Shape pointingCircle;

        KinectManager kinectManager;

        Cell[,] grid;
        Cell target, nextTarget;
        GridSize currentSize;
        int gridHeight, gridWidth;
        public static int sgHeight = 10, sgWidth = 20, lgHeight = sgHeight/2, lgWidth = sgWidth/2;
        //public static int sgHeight = 12, sgWidth = 18, lgHeight = 6, lgWidth = 9;
        //public static int sgHeight = 20, sgWidth = 30, lgHeight = 10, lgWidth = 15;
        double squareHeight = 0, squareWidth = 0;
        static CanvasWindow window;
        TestSuite currentTest;
        List<String> shapes;
        Brush targetColor = Brushes.DarkGray;

        public CanvasWindow() {
            shapes = new List<String>();
            shapes.Add("circle");
            shapes.Add("square");
            GestureParser.Initialize(this);
            currentSize = GridSize.Small;
            window = this;
            InitializeComponent();
            kinectManager = new KinectManager(this);
        }

        public void CreateGrid(GridSize size) {
            if(size == GridSize.Large) {
                CreateGrid(lgWidth,lgHeight);
            } else {
                CreateGrid(sgWidth, sgHeight);
            }
        }

        private void CreateGrid(int width, int height) {
            if(grid != null) {
                canvas.Children.Clear();
            }
            if(pointingCircle != null) {
                canvas.Children.Add(pointingCircle);
            }
            gridHeight = height;
            gridWidth = width;
            squareHeight = canvas.ActualHeight / height;
            squareWidth = canvas.ActualWidth / width;

            grid = new Cell[width, height];

            for(int i = 0; i < width; i++) {
                for(int j = 0; j < height; j++) {
                    grid[i, j] = new Cell(ShapeFactory.CreateGridCell(squareWidth, squareHeight));
                    grid[i, j].X = j; grid[i, j].Y = i;
                    canvas.Children.Add(grid[i, j].GridCell);
                    Canvas.SetBottom(grid[i, j].GridCell, j * squareHeight);
                    Canvas.SetLeft(grid[i, j].GridCell, i * squareWidth);
                    Canvas.SetZIndex(grid[i, j].GridCell, 0);
                }
            }
        }

        private Random randomizer = new Random();

        public void CreatePushTarget(int x, int y)
        {
            nextTarget = GetCell(new Point(x,y));
            
        }

        bool runningTest = false;
        public void DrawNextTargets() {
            if (runningTest) {
                if(target == null) {
                    double size = squareWidth > squareHeight ? squareHeight : squareWidth;
                    string shape = shapes[randomizer.Next(shapes.Count)];

                    target = nextTarget;
                    target.GridCell.Fill = targetColor;
                    PushShape(shape, target);

                    target.Shape.Fill = Brushes.Black;

                }
            }
        }

        public void CreatePullTargets(int x1, int y1, int x2, int y2) {
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
        }
        
        protected Point pointer = new Point();

        public void PointAt(double xFromMid, double yFromMid) {

            if (pointingCircle == null) {
                pointingCircle = ShapeFactory.CreatePointer();
                canvas.Children.Add(pointingCircle);
                Canvas.SetZIndex(pointingCircle, 10000);
            }
            if (target != null) {
                target.GridCell.Fill = targetColor;
            }

            DrawNextTargets();

            pointer = GetPoint(xFromMid, yFromMid);
            MoveShape(pointingCircle, pointer);
            ColorCell(pointer);
            KinectGesture gesture = GestureParser.AwaitingGesture;

            if(gesture != null) {
                Cell currCell = GetCell(pointer);
                if(currCell == target) {
                    currentTest.TargetHit(true);
                    PushShape(gesture.Shape, currCell);
                    TargetHit(currCell, true);
                }
                else {
                    currentTest.TargetHit(false);
                    PushShape(gesture.Shape, currCell);
                    TargetHit(currCell, false);
                }
            } 
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
            if (hit) {
                cell.Shape.Fill = Brushes.Green;
            }
            else {
                cell.Shape.Fill = Brushes.Red;
                canvas.Children.Remove(target.Shape);
                target.Shape = null;
            }
            DoubleAnimation da = new DoubleAnimation(0, TimeSpan.FromSeconds(1));
            da.Completed += Da_Completed;
            targetColor = Brushes.White;
            cell.Shape.BeginAnimation(Canvas.OpacityProperty, da);
        }
        private void Da_Completed(object sender, EventArgs e) {
            Cell t = target;
            target = null;
            t.GridCell.Fill = Brushes.White;
            targetColor = Brushes.DarkGray;
        }

        private void MoveShape(Shape shapeToMove, Point p) {

            double x = p.X - (shapeToMove.Width / 2);
            double y = p.Y - (shapeToMove.Height / 2);
            if (Double.IsNaN(shapeToMove.Width) || Double.IsNaN(shapeToMove.Height)) {
                double size = squareWidth < squareHeight ? squareWidth : squareHeight;
                x = p.X - (size / 2);
                y = p.Y - (size / 2);
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
        }

        public Point GetPoint(double xFromMid, double yFromMid)
        {
            double x = Scale(canvas.ActualWidth, .25f, xFromMid);
            double y = Scale(canvas.ActualHeight, .26f, yFromMid);
            Point p = new Point(x, y);

            return p;
        }

        public void StopTest() {
            runningTest = false;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if(e.Key == System.Windows.Input.Key.Up) {
                currentTest = new TestSuite(GestureDirection.Push, this);
                runningTest = true;
            } else if (e.Key == System.Windows.Input.Key.Down) {
                currentTest = new TestSuite(GestureDirection.Pull, this);
                runningTest = true;
            } 
            
            else if (e.Key == System.Windows.Input.Key.Q) {
                GestureParser.SetTypeContext(GestureType.Swipe);
            } else if (e.Key == System.Windows.Input.Key.W) {
                GestureParser.SetTypeContext(GestureType.Throw);
            } else if (e.Key == System.Windows.Input.Key.E) {
                GestureParser.SetTypeContext(GestureType.Pinch);
            } else if (e.Key == System.Windows.Input.Key.R) {
                GestureParser.SetTypeContext(GestureType.Tilt);
            } 
            
            else if (e.Key == System.Windows.Input.Key.A) {
                GestureParser.SetDirectionContext(GestureDirection.Push);
            } else if (e.Key == System.Windows.Input.Key.S) {
                GestureParser.SetDirectionContext(GestureDirection.Pull);
            }
            else if(e.Key == System.Windows.Input.Key.Z) {
                CreateGrid(GridSize.Small);
            }else if(e.Key == System.Windows.Input.Key.X) {
                CreateGrid(GridSize.Large);
            }
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
