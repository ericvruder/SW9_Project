using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SW9_Project {
    /// <summary>
    /// Interaction logic for CanvasWindow.xaml
    /// </summary>
    public partial class CanvasWindow : Window, IDrawingBoard {

        Shape pointingCircle;

        KinectManager kinectManager;

        Cell[,] grid;
        int gridHeight = 20, gridWidth = 20;
        double squareHeight = 0, squareWidth = 0;

        Point pointFromMid = new Point();

        public CanvasWindow() {
            InitializeComponent();
            kinectManager = new KinectManager(this);
        }

        private void CreateGrid() {
            CreateGrid(gridHeight, gridWidth);
        }

        private void CreateGrid(int width, int height) {
            gridHeight = height;
            gridWidth = width;
            squareHeight = canvas.ActualHeight / height;
            squareWidth = canvas.ActualWidth / width;

            grid = new Cell[width, height];

            for(int i = 0; i < width; i++) {
                for(int j = 0; j < height; j++) {
                    grid[i, j] = new Cell(ShapeFactory.CreateGridCell(squareWidth, squareHeight));
                    canvas.Children.Add(grid[i, j].GridCell);
                    Canvas.SetBottom(grid[i, j].GridCell, j * squareHeight);
                    Canvas.SetLeft(grid[i, j].GridCell, i * squareWidth);
                }
            }
        }

        private Cell GetCell(Point p) {


            int x = (int)Math.Floor(p.X / squareWidth);
            int y = (int)Math.Floor(p.Y / squareHeight);

            if (x >= gridWidth) { x = gridWidth - 1; }
            if (y >= gridHeight) { y = gridHeight - 1; }

            return grid[x, y]; 

        }

        Rectangle currentCell;
        private void ColorCell(Point toColor) {
            
            if (currentCell != null) {
                currentCell.Fill = Brushes.Transparent;
            }
            currentCell = GetCell(toColor).GridCell;
            currentCell.Fill = Brushes.Yellow;
        }
        
        Point pointer = new Point();

        public void PointAt(double xFromMid, double yFromMid) {

            if (pointingCircle == null) {
                pointingCircle = ShapeFactory.CreatePointer();
                canvas.Children.Add(pointingCircle);
                Canvas.SetZIndex(pointingCircle, 1);
            }

            pointer = GetPoint(xFromMid, yFromMid);

            MoveShape(pointingCircle, pointer);
            ColorCell(pointer);
            if (GestureParser.AwaitingGesture != null) {
                lock (GestureParser.AwaitingGesture) {
                    if (GestureParser.AwaitingGesture.Direction == GestureDirection.Push) {
                        ReceiveShape("circle", GestureParser.AwaitingGesture.Pointer);
                        GestureParser.AwaitingGesture = null;
                    }
                    else if(GestureParser.AwaitingGesture.Direction == GestureDirection.Pull) {
                        PullShape(GestureParser.AwaitingGesture.Pointer);
                        GestureParser.AwaitingGesture = null;
                    }
                }
            }
            
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
            if(cell.Shape != null) {
                canvas.Children.Remove(cell.Shape);
            }
            double size = squareHeight < squareWidth ? squareHeight : squareWidth;
            Shape t = ShapeFactory.CreateShape(shape, size);
            double x = Canvas.GetLeft(cell.GridCell) + (cell.GridCell.ActualWidth / 2);
            double y = Canvas.GetBottom(cell.GridCell) + (cell.GridCell.ActualHeight / 2);

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
            CreateGrid();
        }

        public Point GetPoint(double xFromMid, double yFromMid)
        {
            double x = Scale(canvas.ActualWidth, .25f, xFromMid);
            double y = Scale(canvas.ActualHeight, .26f, yFromMid);
            Point p = new Point(x, y);

            return p;
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
            CreateGrid();
        }
    }
}
