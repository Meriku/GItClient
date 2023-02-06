using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using Markdig.Syntax.Inlines;
using System;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GItClient
{

    public partial class MainWindow : Window
    {
        public RelayCommand MaximizedMinimizedWindow { get; set; }

        private AnimationController _animationController;
        private GitController _gitController;

        public MainWindow()
        {
            //TODO: delete extramenu button on UI ?
            //TODO: add colors for git commands? 
 
            InitializeComponent();

            _animationController = ControllersProvider.GetAnimationController();
            _gitController = ControllersProvider.GetGitController();

            // AddEventsToInputManager(); 
            //TODO: don't like this; fix bug with hard to close the bar, and make configurable 

            MaximizedMinimizedWindow = new RelayCommand(headerControlBar_MouseLeftDoubleClick);
            headerBorder.InputBindings.Add(new InputBinding(MaximizedMinimizedWindow, new MouseGesture(MouseAction.LeftDoubleClick)));

            WeakReferenceMessenger.Default.Register<MainViewChangedMessage>(this, (r, m) =>
            { ResizeWindow(m); });

            //test
            WeakReferenceMessenger.Default.Register<UpdateGitHistoryMessage>(this, (r, m) =>
            { ColorFormatGitCommands(); });
        }

        private void ColorFormatGitCommands()
        {
            GitCommandsTextBlock.Text = "";
            var text = _gitController.GetUnFormattedCommandsHistory();
            

            foreach (var line in text)
            {
                var words = line.Split(' ');

                for (var i = 0; i < words.Length; i++)
                {
                    GitCommandsTextBlock.Inlines.Add(FormatByPattern(words[i], i));       
                }

                GitCommandsTextBlock.Inlines.Add("\n");
            }
        }

        private Run FormatByPattern(string word, int index)
        {
            word += " ";

            switch (index)
            {
                case 0:
                    return new Run(word) { Foreground = Brushes.DimGray };
                case 1:
                    return new Run(word) { Foreground = Brushes.DarkGoldenrod };
                case 2:
                    return new Run(word) { Foreground = Brushes.SkyBlue };
                case 3:
                    return new Run(word) { Foreground = Brushes.SlateGray };
                default:
                    return new Run(word) { Foreground = Brushes.White };

            }


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

            WeakReferenceMessenger.Default.Send(new UpdateGitHistoryMessage(1));
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
