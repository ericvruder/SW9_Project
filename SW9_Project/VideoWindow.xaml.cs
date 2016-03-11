using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

namespace SW9_Project
{
    /// <summary>
    /// Interaction logic for VideoWindow.xaml
    /// </summary>
    public partial class VideoWindow : Window
    {

        public VideoWindow(GestureDirection direction, GestureType type, bool reopen = false)
        {
            GestureParser.Pause(!reopen);
            InitializeComponent();
            this.Title = type + " " + direction;
            this.Show();

            string videoDirectory = @"techniques/";
            string video = direction.ToString() + "_" + type.ToString() + ".mp4";


            if (Screen.AllScreens.Length > 1) {
                Screen s = reopen ? Screen.AllScreens[0] : Screen.AllScreens[1];
                System.Drawing.Rectangle r = s.WorkingArea;
                this.Top = r.Top;
                this.Left = r.Left;
                this.Topmost = true;
                this.Show();
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            } else {
                Screen s = Screen.AllScreens[0];
                System.Drawing.Rectangle r = s.WorkingArea;
                this.Top = r.Top;
                this.Left = r.Left;
                this.Show();
            }
            
            videoMediaElement.Source = new Uri(CreateAbsolutePathTo(videoDirectory + video), UriKind.Relative);

            if (reopen) {
                this.Activate();
                canvasWindow.Activate();
                videoMediaElement.MediaEnded += (sender, args) => {
                    videoMediaElement.Position = TimeSpan.Zero;
                };
            } else {
                videoMediaElement.MediaEnded += (sender, args) => {
                    this.Close();
                    var t = new VideoWindow(direction, type, true);
                };
            }
        }
        static CanvasWindow canvasWindow;
        public static void SetCanvasWindow(CanvasWindow window) {
            canvasWindow = window;
        }
        
        private static string CreateAbsolutePathTo(string mediaFile) {
            return Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, mediaFile);
        }
        
        private void Window_Activated(object sender, EventArgs e) {
            //canvasWindow.Activate();
        }
    }
}