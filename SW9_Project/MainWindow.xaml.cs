﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SW9_Project {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {


        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public MainWindow() {
            InitializeComponent();

            Task.Factory.StartNew(() => {
                AllocConsole();
                Connection.StartService();
            });

            CanvasWindow canvas = new CanvasWindow();
            if (Screen.AllScreens.Length > 1) {
                Screen s2 = Screen.AllScreens[1];
                System.Drawing.Rectangle r2 = s2.WorkingArea;
                canvas.Top = r2.Top;
                canvas.Left = r2.Left;
                canvas.Show();
                canvas.WindowStyle = WindowStyle.None;
                canvas.WindowState = WindowState.Maximized;
            } else {
                Screen s1 = Screen.AllScreens[0];
                System.Drawing.Rectangle r1 = s1.WorkingArea;
                canvas.Top = r1.Top;
                canvas.Left = r1.Left;
                canvas.Show();
            }

        }
    }
}