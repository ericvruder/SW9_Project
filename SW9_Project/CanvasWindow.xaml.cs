using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SW9_Project {
    /// <summary>
    /// Interaction logic for CanvasWindow.xaml
    /// </summary>
    public partial class CanvasWindow : Window, IDrawingBoard {

        double xScale, yScale;

        Shape pointingCircle;

        KinectManager kinectManager;

        public CanvasWindow() {

            InitializeComponent();

            kinectManager = new KinectManager(this);
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

        public void SetScalingFactor(double x, double y) {
            xScale = x;
            yScale = y;
        }

        public void PointAt(double xFromMid, double yFromMid) {
            if(pointingCircle == null) {
                pointingCircle = CreateEllipse();
            }
            MoveShape(pointingCircle, xFromMid, yFromMid);
        }

        public void PullShape(double xFromMid, double yFromMid) {
            throw new NotImplementedException();
        }

        public void ReceiveShape(Shape shapeToMove, double xFromMid, double yFromMid) {
            throw new NotImplementedException();
        }


        private void MoveShape(Shape shapeToMove, double xFromMid, double yFromMid) {

            double x = (canvas.ActualWidth / 2) - (shapeToMove.Width / 2);
            double y = (canvas.ActualHeight / 2) - (shapeToMove.Height / 2);

            //x += (xScale * xFromMid);
            //y += (yScale * yFromMid);
            x = Scale(xScale, .25f, xFromMid);
            y = Scale(yScale, .26f, yFromMid);

            Canvas.SetLeft(shapeToMove, x);
            Canvas.SetBottom(shapeToMove, y);
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

    }
}
