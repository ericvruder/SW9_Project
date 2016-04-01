using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

using DataSetGenerator;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interop;
using AxAXVLC;

namespace SW9_Project {
    /// <summary>
    /// Interaction logic for VideoWindow.xaml
    /// </summary>
    public partial class VideoWindow : Window {

        static VideoWindow videoWindow;
        AxVLCPlugin2 vlc;

        public static void PlayVideo(GestureDirection direction, GestureType type) {
            videoWindow?.Close();
            videoWindow = new VideoWindow(direction, type);
            canvasWindow.LockScreen(type, direction);
        }

        public VideoWindow(GestureDirection direction, GestureType type) {

            InitializeComponent();
            this.Title = type + " " + direction;

            SetScreen(false);

            vlc = new AxVLCPlugin2();

            formHost.Child = vlc;

            var uri = CreateUriTo(type, direction);
            var convertedURI = uri.AbsoluteUri;
            vlc.CreateControl();
            vlc.playlist.add(convertedURI);
            vlc.playlist.play();
        }
        private void SetScreen(bool primaryScreen) {

            if (Screen.AllScreens.Length > 1) {
                int secScreen = Screen.AllScreens.Length == 2 ? 0 : 2;
                int mainScreen = Screen.AllScreens.Length == 2 ? 1 : 0;
                Screen s = primaryScreen ? Screen.AllScreens[mainScreen] : Screen.AllScreens[secScreen];
                System.Drawing.Rectangle r = s.Bounds;
                Topmost = true;
                Top = r.Top;
                Left = r.Left;
                Width = r.Width;
                Height = r.Height;
                Show();
            }
            else {
                Screen s = Screen.AllScreens[0];
                System.Drawing.Rectangle r = s.WorkingArea;
                Top = r.Top;
                Left = r.Left;
                Show();
            }
            canvasWindow.Activate();

        }
        static CanvasWindow canvasWindow;
        public static void SetCanvasWindow(CanvasWindow window) {
            canvasWindow = window;
        }

        private static Uri CreateUriTo(GestureType type, GestureDirection direction) {

            string videoDirectory = @"techniques/" + direction.ToString() + "_" + type.ToString() + ".mp4";
            string path = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, videoDirectory);
            return new Uri(path);

        }


    }
}