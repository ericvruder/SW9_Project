using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

using DataSetGenerator;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SW9_Project
{
    /// <summary>
    /// Interaction logic for VideoWindow.xaml
    /// </summary>
    public partial class VideoWindow : Window {

        static VideoWindow currentVideoWindow;

        public VideoWindow(GestureDirection direction, GestureType type, bool reopen = false)
        {
            if(currentVideoWindow != null) {
                currentVideoWindow.CloseWindow();
                currentVideoWindow = this;
            }
            else {
                currentVideoWindow = this;
            }
            GestureParser.Pause(!reopen);
            InitializeComponent();
            this.Title = type + " " + direction;
            this.Show();

            videoMediaElement.LoadedBehavior = MediaState.Manual;
            videoMediaElement.UnloadedBehavior = MediaState.Manual;
            string videoDirectory = @"techniques/";
            string video = direction.ToString() + "_" + type.ToString() + ".mp4";


            if (Screen.AllScreens.Length > 1) {
                int secScreen = Screen.AllScreens.Length == 2 ? 0 : 2;
                int mainScreen = Screen.AllScreens.Length == 2 ? 1 : 0;
                Screen s = reopen ? Screen.AllScreens[secScreen] : Screen.AllScreens[mainScreen];
                System.Drawing.Rectangle r = s.WorkingArea;
                this.Top = r.Top;
                this.Left = r.Left;
                this.Topmost = true;
                this.Show();
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
            else {
                Screen s = Screen.AllScreens[0];
                System.Drawing.Rectangle r = s.WorkingArea;
                this.Top = r.Top;
                this.Left = r.Left;
                this.Show();
            }
            String videoPath = CreateAbsolutePathTo(videoDirectory + video);
            if (File.Exists(videoPath)) {
                Uri videoUri = new Uri(videoPath, UriKind.Relative);
                videoMediaElement.Source = videoUri;
            }
            
            if (reopen) {
                this.Activate();
                canvasWindow.Activate();
                videoMediaElement.MediaEnded += (sender, args) => {
                    videoMediaElement.Stop();
                    videoMediaElement.Position = TimeSpan.Zero;
                };
            } else {
                videoMediaElement.MediaEnded += (sender, args) => {
                    this.CloseWindow();
                    var t = new VideoWindow(direction, type, true);
                };
            }
            videoMediaElement.Play();

            Task task = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Started video check");
                while(GetMediaState(videoMediaElement) != MediaState.Play)
                {
                    Console.WriteLine("Video not playing, attempting to play");
                    videoMediaElement.Play();
                }
            });
        }

        private MediaState GetMediaState(MediaElement myMedia)
        {
            FieldInfo hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
            object helperObject = hlp.GetValue(myMedia);
            FieldInfo stateField = helperObject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            MediaState state = (MediaState)stateField.GetValue(helperObject);
            return state;
        }

        static CanvasWindow canvasWindow;
        public static void SetCanvasWindow(CanvasWindow window) {
            canvasWindow = window;
        }
        
        private static string CreateAbsolutePathTo(string mediaFile) {
            return Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, mediaFile);
        }

        private void CloseWindow() {
            videoMediaElement.Stop();
            videoMediaElement.Close();
            this.Close();
        }

    }
}