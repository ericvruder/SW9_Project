using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SW9_Project {
    public partial class MainForm : Form {

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public MainForm() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            Form objectTracker = new ObjectTrackerForm();
            objectTracker.Show();
        }

        private void button2_Click(object sender, EventArgs e) {
            AllocConsole();
            Console.WriteLine("Testing");
            Server t = new Server();
        }
    }
}
