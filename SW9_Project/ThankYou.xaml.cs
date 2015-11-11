using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SW9_Project {
    /// <summary>
    /// Interaction logic for ThankYou.xaml
    /// </summary>
    public partial class ThankYou : Window {
        public ThankYou() {
            InitializeComponent();

            if (Screen.AllScreens.Length > 1) {
                Screen s2 = Screen.AllScreens[1];
                System.Drawing.Rectangle r2 = s2.WorkingArea;
                this.Top = r2.Top;
                this.Left = r2.Left;
                this.Show();
                this.WindowState = WindowState.Maximized;
            } else {
                Screen s1 = Screen.AllScreens[0];
                System.Drawing.Rectangle r1 = s1.WorkingArea;
                this.Top = r1.Top;
                this.Left = r1.Left;
                this.Show();
            }
        }
    }
}
