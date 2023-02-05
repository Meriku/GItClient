using CommunityToolkit.Mvvm.Input;
using GItClient.Core;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace GItClient
{

    public partial class MainWindow : Window
    {
        public RelayCommand MaximizedMinimizedWindow { get; set; }

        public MainWindow()
        {
            //TODO: delete extramenu button on UI ?

            InitializeComponent();

            MaximizedMinimizedWindow = new RelayCommand(headerControlBar_MouseLeftDoubleClick);
            headerBorder.InputBindings.Add(new InputBinding(MaximizedMinimizedWindow, new MouseGesture(MouseAction.LeftDoubleClick)));

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
