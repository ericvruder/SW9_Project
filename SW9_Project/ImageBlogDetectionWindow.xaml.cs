using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SW9_Project {
    /// <summary>
    /// Interaction logic for ImageBlogDetection.xaml
    /// </summary>
    public partial class ImageBlogDetectionWindow : Window {
        public ImageBlogDetectionWindow() {
            InitializeComponent();
        }

        private void selectImageButton_Click(object sender, RoutedEventArgs e) {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            string filename;

            // Get the selected file name and display in a TextBox 
            if (result == true) {
                filename = dlg.FileName;
            }
            else {
                filename = "testing.png";
            }
        }
    }
}
