using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

using DataSetGenerator;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interop;

namespace SW9_Project
{
    /// <summary>
    /// Interaction logic for VideoWindow.xaml
    /// </summary>
    public partial class VideoWindow : Window {

        static VideoWindow currentVideoWindow;

        public VideoWindow() {
            InitializeComponent();
        }
        static bool softwareAccSet = true;
        public static void PlayVideo(GestureDirection direction, GestureType type) {
            GestureParser.Pause(true);
            if(currentVideoWindow == null) {
                currentVideoWindow = new VideoWindow();
            }
            else if(!softwareAccSet){
                //softwareAccSet = true;
                HwndSource hwndSource = PresentationSource.FromVisual(currentVideoWindow) as HwndSource;
                HwndTarget hwndTarget = hwndSource.CompositionTarget;
                hwndTarget.RenderMode = RenderMode.SoftwareOnly;
            }
            
            string videoPath = GetVideoPath(direction, type);
            if (File.Exists(videoPath)) {
                currentVideoWindow.videoMediaElement.Source = new Uri(videoPath, UriKind.Relative);
                currentVideoWindow.videoMediaElement.Position = TimeSpan.Zero;
                currentVideoWindow.MoveWindow(true);

                RoutedEventHandler handler = null;

                handler = (sender, e) => {
                    GestureParser.Pause(false);
                    currentVideoWindow.videoMediaElement.Position = TimeSpan.Zero; 
                    
                    currentVideoWindow.videoMediaElement.MediaEnded -= handler;
                    currentVideoWindow.MoveWindow(false);
                    currentVideoWindow.videoMediaElement.MediaEnded += (senderI, eI) => {
                        currentVideoWindow.videoMediaElement.Position = TimeSpan.Zero;
                    };
                    canvasWindow.Activate();
                };

                currentVideoWindow.videoMediaElement.MediaEnded += handler;

                
            }
        }
        



        static CanvasWindow canvasWindow;
        public static void SetCanvasWindow(CanvasWindow window) {
            canvasWindow = window;
        }

        private void MoveWindow(bool primaryScreen) {

            if (Screen.AllScreens.Length > 1) {
                int secScreen = Screen.AllScreens.Length == 2 ? 0 : 2;
                int mainScreen = Screen.AllScreens.Length == 2 ? 1 : 0;
                Screen s = primaryScreen ? Screen.AllScreens[mainScreen] : Screen.AllScreens[secScreen];
                System.Drawing.Rectangle r = s.Bounds;
                this.Topmost = true;
                this.Top = r.Top;
                this.Left = r.Left;
                this.Width = r.Width;
                this.Height = r.Height;
                this.Show();
                this.Activate();
            }
            else {
                Screen s = Screen.AllScreens[0];
                System.Drawing.Rectangle r = s.WorkingArea;
                this.Top = r.Top;
                this.Left = r.Left;
                this.Show();
            }

        }
        
        private static string GetVideoPath(GestureDirection direction, GestureType type) {

            string videoDirectory = @"techniques/";
            string video = direction.ToString() + "_" + type.ToString() + ".wmv";

            return Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, videoDirectory + video);
        }

    }
}