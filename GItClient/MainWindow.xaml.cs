using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using System;
using System.Management.Automation.Runspaces;
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

        private AnimationController _animationController;

        public MainWindow()
        {
            //TODO: delete extramenu button on UI ?
            //TODO: add colors for git commands? 

            InitializeComponent();

            MaximizedMinimizedWindow = new RelayCommand(headerControlBar_MouseLeftDoubleClick);
            headerBorder.InputBindings.Add(new InputBinding(MaximizedMinimizedWindow, new MouseGesture(MouseAction.LeftDoubleClick)));

            WeakReferenceMessenger.Default.Register<MainViewChangedMessage>(this, (r, m) =>
            { ResizeWindow(m); });

            _animationController = ControllersProvider.GetAnimationController();
        }



        private void openCloseGitCommandsBar(object sender = null, EventArgs e = null)
        {
            var animations = _animationController.GetCommandsBarAnimation(GitCommandsButton.ActualHeight, GitCommandsButton.ActualWidth, false);
            WeakReferenceMessenger.Default.Send(new GitCommandsHistoryMessage(10));

            GitCommandsButton.BeginAnimation(HeightProperty, animations.Height);
            GitCommandsButton.BeginAnimation(WidthProperty, animations.Width);

            
        }


        private const int MarginHeight = 60;
        private const int MarginWidth = 15;
        private const int MenuMinHeight = 130;
        private void ResizeWindow(MainViewChangedMessage message)
        {
            var window = Application.Current.MainWindow;
            window.MinHeight = message.Value.MinHeight + MarginHeight;
            window.MinWidth = message.Value.MinWidth + MenuMinHeight + MarginWidth;
        }


        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void headerControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_animationController.IsCommandsBarOpen)
            {
                var animations = _animationController.GetCommandsBarAnimation(GitCommandsButton.ActualHeight, GitCommandsButton.ActualWidth, true);

                GitCommandsButton.BeginAnimation(HeightProperty, animations.Height);
                GitCommandsButton.BeginAnimation(WidthProperty, animations.Width);
            }
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
