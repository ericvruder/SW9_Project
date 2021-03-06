﻿using SW9_Project.Logging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using DataSetGenerator;
using System.Diagnostics;

namespace SW9_Project {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        private bool isBuletinBoard = false;


        public MainWindow() {
            //Somestuff
            AttemptRepository.SaveStatus = DatabaseSaveStatus.Success;
            InitializeComponent();
            Task.Factory.StartNew(() => {
                AllocConsole();
                Connection.StartService();
            });
            
            Target.Initialize();

            StartCanvasWindow();
            
        }

        private void StartCanvasWindow() {
            CanvasWindow canvas = new CanvasWindow();
            if (Screen.AllScreens.Length > 1) {
                int mainScreen = Screen.AllScreens.Length == 2 ? 1 : 0;
                Screen s2 = Screen.AllScreens[mainScreen];
                System.Drawing.Rectangle r2 = s2.Bounds;
                canvas.Top = r2.Top;
                canvas.Left = r2.Left;
                canvas.Show();
                canvas.WindowStyle = WindowStyle.None;
                canvas.WindowState = WindowState.Maximized;
                canvas.Topmost = true;
            }
            else {
                Screen s1 = Screen.AllScreens[0];
                System.Drawing.Rectangle r1 = s1.WorkingArea;
                canvas.Top = r1.Top;
                canvas.Left = r1.Left;
                canvas.Show();
            }
        }
    }
}
