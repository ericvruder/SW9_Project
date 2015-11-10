using System;
using System.IO;
using System.Reflection;
using System.Windows;

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

            videoMediaElement.Source = new Uri(CreateAbsolutePathTo(videoDirectory + video), UriKind.Relative);

            videoMediaElement.MediaEnded += (sender, args) =>
            {
                this.Hide();
            };
        }
        private static string CreateAbsolutePathTo(string mediaFile) {
            return Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, mediaFile);
        }

    }
}