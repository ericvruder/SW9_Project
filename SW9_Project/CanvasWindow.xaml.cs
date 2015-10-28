using System;
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

        Rectangle[,] grid;
        int gridHeight = 10, gridWidth = 10;
        double squareHeight = 0, squareWidth = 0;

        double xScale = 2000, yScale = 2000;

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

            grid = new Rectangle[width, height];

            for(int i = 0; i < width; i++) {
                for(int j = 0; j < height; j++) {
                    grid[i, j] = CreateRectangle(squareWidth, squareHeight);
                    Canvas.SetTop(grid[i, j], j * squareHeight);
                    Canvas.SetLeft(grid[i, j], i * squareWidth);
                }
            }
        }

        Rectangle currentCell;
        private void ColorCell(double height, double width) {
            int x = (int)Math.Ceiling(width / squareWidth);
            int y = (int)Math.Ceiling(height / squareHeight);
            if (currentCell != null) {
                currentCell.Fill = Brushes.Transparent;
            }
            currentCell = grid[x, y];
            currentCell.Fill = Brushes.LightYellow;
        }

        public Shape CreateEllipse() {

            Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.Red;
            ellipse.StrokeThickness = 1;
            ellipse.Stroke = Brushes.Black;
            ellipse.Height = 100;
            ellipse.Width = 100;

            canvas.Children.Add(ellipse);

            return ellipse;
        }

        private Rectangle CreateRectangle(double width, double height) {
            Rectangle rectangle = new Rectangle();
            rectangle.StrokeThickness = 1;
            rectangle.Fill = Brushes.Transparent;
            rectangle.Stroke = Brushes.Black;
            rectangle.Height = height;
            rectangle.Width = width;

            canvas.Children.Add(rectangle);

            return rectangle;
        }

        Point pointFromMid = new Point();

        public void PointAt(double xFromMid, double yFromMid) {

            if (pointingCircle == null) {
                pointingCircle = CreateEllipse();
            }

            pointFromMid = GetPoint(xFromMid, yFromMid);

            MoveShape(pointingCircle, pointFromMid.X, pointFromMid.Y);
            ColorCell(pointFromMid.X, pointFromMid.Y);
            
        }

        public void PullShape(double xFromMid, double yFromMid) {
            throw new NotImplementedException();
        }

        public void ReceiveShape(Shape shapeToMove, double xFromMid, double yFromMid) {
            throw new NotImplementedException();
        }

        private void MoveShape(Shape shapeToMove, double xFromMid, double yFromMid) {

            double x = xFromMid - (shapeToMove.Width / 2);
            double y = yFromMid - (shapeToMove.Height / 2);
            
            Canvas.SetLeft(shapeToMove, x);
            Canvas.SetBottom(shapeToMove, y);
        }

        public Point GetPoint(double xFromMid, double yFromMid)
        {
            double x = Scale(xScale, .25f, xFromMid);
            double y = Scale(yScale, .26f, yFromMid);
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
