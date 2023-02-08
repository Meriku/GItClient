using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;

namespace GItClient
{

    public partial class MainWindow : Window
    {
        public RelayCommand MaximizedMinimizedWindow { get; set; }

        private AnimationController _animationController;
        private GitController _gitController;
        private TextController _textController;

        public MainWindow()
        {
            // TODO: delete extramenu button on UI ?
            // TODO: add colors for git commands? 
            // TODO: AddEventsToInputManager() don't like it;
            // fix bug with hard to close the bar, and make configurable 

            InitializeComponent();

            MaximizedMinimizedWindow = new RelayCommand(headerControlBar_MouseLeftDoubleClick);
            headerBorder.InputBindings.Add(new InputBinding(MaximizedMinimizedWindow, new MouseGesture(MouseAction.LeftDoubleClick)));

            _animationController = ControllersProvider.GetAnimationController();
            _gitController = ControllersProvider.GetGitController();
            _textController = ControllersProvider.GetTextController();

            WeakReferenceMessenger.Default.Register<MainViewChangedMessage>(this, (r, m) =>
            { ResizeWindow(m); });

            WeakReferenceMessenger.Default.Register<UpdateGitHistoryMessage>(this, (r, m) =>
            { UpdateGitBarTextSafely(); });
        }


        private async void UpdateGitBarTextSafely()
        {
            var text = await Task.Run(() => _gitController.GetUnFormattedCommandsHistory());
            // Possible block of UI, so executed in a separate Thread

            this.Dispatcher.Invoke(() => 
            {
                var lines = _textController.GetInlines(text);
                GitCommandsTextBlock.Inlines.Clear();
                GitCommandsTextBlock.Inlines.AddRange(lines);
            });
        }



        private void AddEventsToInputManager()
        {
            // TODO: make close git comands window on click anywhere configurable?
            InputManager.Current.PreProcessInput += (sender, e) =>
            {
                if (e.StagingItem.Input is MouseButtonEventArgs args && args.ClickCount > 0)
                {
                    if (_animationController.IsCommandsBarOpen)
                    {
                        openCloseGitCommandsBar();
                    };
                };
            };
        }

        private void openCloseGitCommandsBar(object sender = null, EventArgs e = null)
        {
            var animations = _animationController.GetCommandsBarAnimation(GitCommandsButton.ActualHeight, GitCommandsButton.ActualWidth, false);

            GitCommandsButton.BeginAnimation(HeightProperty, animations.Height);
            GitCommandsButton.BeginAnimation(WidthProperty, animations.Width);
        }


        private const int MarginHeight = 60;
        private const int MarginWidth = 30;
        private const int MenuMinHeight = 130;
        private void ResizeWindow(MainViewChangedMessage message)
        {
            var window = System.Windows.Application.Current.MainWindow;
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
