using CommunityToolkit.Mvvm.Input;
using GItClient.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace GItClient
{

    public partial class MainWindow : Window
    {
        public RelayCommand MaximizedMinimizedWindow { get; set; }

        public MainWindow()
        {
            //TODO: delete extramenu button on UI ?
            //TODO: add colors for git commands? 

            InitializeComponent();

            MaximizedMinimizedWindow = new RelayCommand(headerControlBar_MouseLeftDoubleClick);
            headerBorder.InputBindings.Add(new InputBinding(MaximizedMinimizedWindow, new MouseGesture(MouseAction.LeftDoubleClick)));

        }

        private void gitHistory_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GitCommandsBorder.Height == 35)
            {
                // TODO: smooth animation, if possible? 
                // TODO: load git commands history

                GitCommandsBorder.Height = 300;
                GitCommandsBorder.Opacity = 0.98;
            }
            else
            {
                GitCommandsBorder.Opacity = 1;
                GitCommandsBorder.Height = 35;
            }
        }


        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void headerControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }

        private void headerControlBar_MouseLeftDoubleClick()
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
            else
                this.WindowState = System.Windows.WindowState.Maximized;
        }

    }
}
