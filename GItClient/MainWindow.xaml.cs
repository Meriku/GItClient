using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core;
using GItClient.Core.Models;
using GItClient.MVVM.ViewModel;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GItClient
{

    public partial class MainWindow : Window
    {
        public RelayCommand MaximizedMinimizedWindow { get; set; }
        private bool IsGitCommandsBarOpen;

        public MainWindow()
        {
            //TODO: delete extramenu button on UI ?
            //TODO: add colors for git commands? 

            InitializeComponent();

            MaximizedMinimizedWindow = new RelayCommand(headerControlBar_MouseLeftDoubleClick);
            headerBorder.InputBindings.Add(new InputBinding(MaximizedMinimizedWindow, new MouseGesture(MouseAction.LeftDoubleClick)));

            WeakReferenceMessenger.Default.Register<MainViewChangedMessage>(this, (r, m) =>
            { ResizeWindow(m); });
        }



        private void gitCommandsButton_MouseLeftButtonDown(object sender, EventArgs e)
        {
            // very bad unclean code for testing purposes
            // after thousand tries to implement this bar using WPF,
            // decided to do it here. 
            // btw this approach use in 10 times less code and more flexible

            var startHeight = 0.0;
            var endHeight = 0.0;

            var startWidth = 0.0;
            var endWidth = 0.0;

            var GitCommandsBarMaxHeight = (int)Math.Round(Application.Current.MainWindow.ActualHeight / 1.80, 0);
            var GitCommandsBarMaxWidth = (int)Math.Round(Application.Current.MainWindow.ActualWidth / 1.80, 0);

            if (GitCommandsButton.Height == 30)
            {
                startHeight = 30;
                endHeight = GitCommandsBarMaxHeight;

                startWidth = 250;
                endWidth = GitCommandsBarMaxWidth > 250 ? GitCommandsBarMaxWidth : 250;

                IsGitCommandsBarOpen = true;
            }
            else if (GitCommandsButton.Height > 30)
            {
                startHeight = GitCommandsButton.Height;
                endHeight = 30;

                startWidth = GitCommandsButton.Width;
                endWidth = 250;

                IsGitCommandsBarOpen = false;
            }

            var AnimationHeight = new DoubleAnimation()
            {
                Duration = new Duration(new TimeSpan(0,0,0,0,300)),
                From = startHeight,
                To = endHeight
            };

            var AnimationWidth = new DoubleAnimation()
            {
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300)),
                From = startWidth,
                To = endWidth
            };

            GitCommandsButton.BeginAnimation(HeightProperty, AnimationHeight);
            GitCommandsButton.BeginAnimation(WidthProperty, AnimationWidth);


        }


        private const int MarginHeight = 60;
        private const int MarginWidth = 15;
        private const int MenuMinHeight = 130;
        private void ResizeWindow(MainViewChangedMessage message)
        {
            var window = Application.Current.MainWindow;
            window.MinHeight = message.Value.MinHeight + MarginHeight;
            window.MinWidth = message.Value.MinWidth + MenuMinHeight + MarginWidth;

            if (IsGitCommandsBarOpen)
            {
                gitCommandsButton_MouseLeftButtonDown(null, null);
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
