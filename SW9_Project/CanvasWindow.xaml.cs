using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using SW9_Project.Logging;

namespace SW9_Project {
    /// <summary>
    /// Interaction logic for CanvasWindow.xaml
    /// </summary>
    public partial class CanvasWindow : Window, IDrawingBoard {

        Shape pointingCircle;

        KinectManager kinectManager;

        Cell[,] grid;
        Cell target;
        GridSize currentSize;
        int gridHeight, gridWidth;
        int sgHeight = 20, sgWidth = 30, lgHeight = 10, lgWidth = 15;
        double squareHeight = 0, squareWidth = 0;
        static CanvasWindow window;

        public CanvasWindow() {
            currentSize = GridSize.Small;
            window = this;
            InitializeComponent();
            kinectManager = new KinectManager(this);
        }

        private void AddTestShapes() {
            for(int i = 105; i < canvas.ActualWidth; i += 200) {
                for(int j = 104; j < canvas.ActualHeight; j += 200) {
                    ReceiveShape("circle", new Point(i, j));
                }
            }
        }

        private void CreateGrid(GridSize size) {
            if(size == GridSize.Large) {
                CreateGrid(lgWidth,lgHeight);
            } else {
                CreateGrid(sgWidth, sgHeight);
            }
            AddTestShapes();
        }

        private void CreateGrid(int width, int height) {
            if(grid != null) {
                canvas.Children.Clear();
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
                    Canvas.SetZIndex(grid[i, j].GridCell, 5);
                }
            }
        }

        private void CreateTarget()
        {
            Random randomizer = new Random();
            target = GetCell(new Point(randomizer.Next((int)canvas.ActualWidth), randomizer.Next((int)canvas.ActualHeight)));
            target.GridCell.Fill = Brushes.Purple;
        }

        public Cell GetCell(Point p) {


            int x = (int)Math.Floor(p.X / squareWidth);
            int y = (int)Math.Floor(p.Y / squareHeight);

            if (x >= gridWidth) { x = gridWidth - 1; }
            if (y >= gridHeight) { y = gridHeight - 1; }

            return grid[x, y]; 

        }

        private enum GridSize { Small, Large }

        Rectangle currentCell;
        private void ColorCell(Point toColor) {
            
            if (currentCell != null) {
                currentCell.Fill = Brushes.Transparent;
            }
            currentCell = GetCell(toColor).GridCell;
            currentCell.Fill = Brushes.Yellow;
        }
        
        protected Point pointer = new Point();

        public void PointAt(double xFromMid, double yFromMid) {

            if (pointingCircle == null) {
                pointingCircle = ShapeFactory.CreatePointer();
                canvas.Children.Add(pointingCircle);
                Canvas.SetZIndex(pointingCircle, 1);
            }
            if (target != null) {
                target.GridCell.Fill = Brushes.Purple;
            }

            pointer = GetPoint(xFromMid, yFromMid);
            MoveShape(pointingCircle, pointer);
            ColorCell(pointer);
            KinectGesture gesture = GestureParser.AwaitingGesture;
            if (gesture?.Direction == GestureDirection.Push) {
                ReceiveShape(gesture.Shape, gesture.Pointer);
            }
            else if(gesture?.Direction == GestureDirection.Pull) {
                PullShape(gesture.Pointer);
            }
            if (gesture != null) {
                CheckForTestChanges();
            }
        }

        private void CheckForTestChanges() {
            Cell currCell = GetCell(pointer);
            if(currCell == target) {
                target.GridCell.Fill = Brushes.Transparent;

                Logger.CurrentLogger.CurrentTargetHit();
                TestSuite.CurrentTest.TargetHit();

                if (TestSuite.CurrentTest.Done) {
                    TestSuite.CurrentTest = null;
                    Logger.CurrentLogger.EndUser();
                    return;
                }

                if (TestSuite.CurrentTest.NextSize) {
                    Logger.CurrentLogger.EndCurrentSizeTest();
                    CreateGrid(GridSize.Large);
                    Logger.CurrentLogger.StartNewSizeTest(gridHeight, gridWidth, squareHeight, squareWidth);
                }
                if (TestSuite.CurrentTest.NextGesture) {
                    Logger.CurrentLogger.EndCurrentGestureTest();
                    Logger.CurrentLogger.StartNewgestureTest(GestureParser.GetTypeContext(), GestureParser.GetDirectionContext());
                }

                Logger.CurrentLogger.AddNewTarget("circle", target.X, target.Y);
                CreateTarget();

            }
        }

        private void StartNewTest() {
            TestSuite.StartNewTest(GestureDirection.Pull);
            CreateTarget();
        }

        public static Point GetCurrentPoint() {
            return window.pointer;
        }

        public Shape PullShape(Point p) {
            Cell cell = GetCell(p);
            Shape returnShape = cell.Shape;
            canvas.Children.Remove(returnShape);
            cell.Shape = null;
            return returnShape;
        }

        public void ReceiveShape(string shape, Point p) {
            Cell cell = GetCell(p);
            canvas.Children.Remove(cell?.Shape);
            double size = squareHeight < squareWidth ? squareHeight : squareWidth;
            Shape t = ShapeFactory.CreateShape(shape, size);
            double x = Canvas.GetLeft(cell.GridCell) + (cell.GridCell.Width / 2);
            double y = Canvas.GetBottom(cell.GridCell) + (cell.GridCell.Height / 2);

            canvas.Children.Add(t);
            cell.Shape = t;

            Point f = new Point(x, y);
            MoveShape(cell.Shape, f);
            

        }

        private void MoveShape(Shape shapeToMove, Point p) {

            double x = p.X - (shapeToMove.Width / 2);
            double y = p.Y - (shapeToMove.Height / 2);
            if (shapeToMove.Width == 0 || shapeToMove.Height == 0) {
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
        }

        public Point GetPoint(double xFromMid, double yFromMid)
        {
            double x = Scale(canvas.ActualWidth, .25f, xFromMid);
            double y = Scale(canvas.ActualHeight, .26f, yFromMid);
            Point p = new Point(x, y);

            return p;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if(e.Key == System.Windows.Input.Key.Space) {
                StartNewTest();
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
    }
}
