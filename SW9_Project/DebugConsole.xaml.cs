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
    /// Interaction logic for DebugConsole.xaml
    /// </summary>
    public partial class DebugConsole : Window {
        static DebugConsole mainConsole;
        public DebugConsole() {
            mainConsole = this;
            InitializeComponent();
            consoleTextBlock.Text += "TESTING!";
        }

        public static void SetRGBFrame(BitmapSource image) {
            mainConsole.rgbKinect.Source = image;
        }
    }
}
