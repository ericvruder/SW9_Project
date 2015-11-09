using System;
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

            string videoDirectory = @"C:\Users\bml\Desktop\techniques_videos-inst\";
            string video = direction.ToString() + "_" + type.ToString() + ".mp4";

            videoMediaElement.Source = new Uri(videoDirectory+video, UriKind.Absolute);

            videoMediaElement.MediaEnded += (sender, args) =>
            {
                this.Hide();
            };
        }

    }
}