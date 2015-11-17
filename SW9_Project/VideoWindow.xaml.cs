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
        public bool DoneShowing { get; set; }

        public VideoWindow(GestureDirection direction, GestureType type)
        {

            InitializeComponent();
            this.Title = type + " " + direction;
            DoneShowing = false;
            this.Show();

            string videoDirectory = @"techniques/";
            string video = direction.ToString() + "_" + type.ToString() + ".mp4";


            if (Screen.AllScreens.Length > 1) {
                Screen s2 = Screen.AllScreens[0];
                System.Drawing.Rectangle r2 = s2.WorkingArea;
                this.Top = r2.Top;
                this.Left = r2.Left;
                this.Show();
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            } else {
                Screen s1 = Screen.AllScreens[0];
                System.Drawing.Rectangle r1 = s1.WorkingArea;
                this.Top = r1.Top;
                this.Left = r1.Left;
                this.Show();
            }
            videoMediaElement.Source = new Uri(CreateAbsolutePathTo(videoDirectory + video), UriKind.Relative);

            videoMediaElement.MediaEnded += (sender, args) =>
            {
                videoMediaElement.Position = TimeSpan.Zero;
            };
        }
        
        private static string CreateAbsolutePathTo(string mediaFile) {
            return Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, mediaFile);
        }

    }
}