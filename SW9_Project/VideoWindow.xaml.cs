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
        }

        public VideoWindow(GestureDirection direction, GestureType type) {

            InitializeComponent();
            this.Title = type + " " + direction;

            vlc = new AxVLCPlugin2();
            formHost.Child = vlc;
            vlc.CreateControl();


            var videoPath = CreateUriTo(type, direction);
            MoveScreen(true);
            GestureParser.Pause(true);
            if (!GlobalVars.isTargetPractice)
            {   this.WindowState = WindowState.Maximized;}
            vlc.playlist.add(videoPath);
            vlc.playlist.play();

            EventHandler handler = null;

            handler = (sender, e) => {
                vlc.MediaPlayerEndReached -= handler;
                if (!GlobalVars.isTargetPractice)
                { this.WindowState = WindowState.Normal; }
                GestureParser.Pause(false);
                MoveScreen(false);
                if (!GlobalVars.isTargetPractice)
                { this.WindowState = WindowState.Maximized; }
                canvasWindow.Activate();
                vlc.playlist.play();
                vlc.MediaPlayerEndReached += (senderI, eI) => {
                    vlc.playlist.play();
                };

            };

            vlc.MediaPlayerEndReached += handler;
        }

        private void MoveScreen(bool primaryScreen) {

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

        }
        static CanvasWindow canvasWindow;
        public static void SetCanvasWindow(CanvasWindow window) {
            canvasWindow = window;
        }

        private static string CreateUriTo(GestureType type, GestureDirection direction) {

            string videoDirectory = @"techniques/" + direction.ToString() + "_" + type.ToString() + ".mp4";
            string path = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, videoDirectory);
            return new Uri(path).AbsoluteUri;

        }


    }
}