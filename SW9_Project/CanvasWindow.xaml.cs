using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SW9_Project {
    /// <summary>
    /// Interaction logic for CanvasWindow.xaml
    /// </summary>
    public partial class CanvasWindow : Window, IDrawingBoard {

        Shape pointingCircle;

        KinectManager kinectManager;

        public CanvasWindow(bool debugging) {

            InitializeComponent();

            if (!debugging) { kinectManager = new KinectManager(this); }
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

        private bool shown = false;

        public void PointAt(double xFromMid, double yFromMid) {

            if (pointingCircle == null) {
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

            x += xFromMid;
            y += yFromMid;

            Canvas.SetLeft(shapeToMove, x);
            Canvas.SetBottom(shapeToMove, y);
        }

        public new double Height() {
            return canvas.ActualHeight;
        }

        public new double Width() {
            return canvas.ActualWidth;
        }

        public void AddDot(double x, double y) {
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.Blue;
            ellipse.StrokeThickness = 0.5;
            ellipse.Stroke = Brushes.Black;
            ellipse.Height = 100;
            ellipse.Width = 100;
            canvas.Children.Add(ellipse);
            MoveShape(ellipse, x, y);
        }
    }
}
