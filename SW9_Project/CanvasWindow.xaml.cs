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

        private bool shown = false;

        public void PointAt(double xFromMid, double yFromMid) {

            if (!shown) {
                shown = true;
                DrawNotice("Testing", 10);
            }

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

        public void DrawNotice(string message, int secondsToShow) {
            DispatcherTimer t = new DispatcherTimer();

            t.Interval = TimeSpan.FromSeconds(secondsToShow);

            //FontSize = "66.667" Foreground = "#FFF31616" FontWeight = "Bold" />
            Label notice = new Label();
            notice.Content = message;
            notice.FontSize = 70.0;
            notice.Foreground = Brushes.DarkRed;
            notice.FontWeight = FontWeights.Bold;
            canvas.Children.Add(notice);
            Canvas.SetBottom(notice, (canvas.ActualHeight / 2) + (notice.ActualHeight /2));
            Canvas.SetLeft(notice, (canvas.ActualWidth / 2) + (notice.ActualWidth /2));

            t.Tick += (sender, e) => NoticeTimeElapse(sender, e, notice);
            //t.Elapsed += (sender, e) => NoticeTimeElapse(sender, e, notice);
            t.Start();
        }

        private void NoticeTimeElapse(object sender, EventArgs e, Label notice) {

            Action RemoveChild = () => { canvas.Children.Remove(notice); };
            Dispatcher.BeginInvoke(RemoveChild, DispatcherPriority.Send, null);

            /*
            this.Dispatcher.InvokeAsync((Action)(() => {
                canvas.Children.Remove(notice);
            }));
            */
        }
    }
}
