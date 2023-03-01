using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Controllers.Static;
using GItClient.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GItClient
{

    public partial class MainWindow : Window
    {
        public RelayCommand MaximizedMinimizedWindow { get; set; }

        private AnimationController _animationController;
        private GitController _gitController;
        private TextController _textController;

        private ILogger _logger = LoggerProvider.GetLogger("MainWindow");
        private readonly SemaphoreSlim _semaphore;        

        public MainWindow()
        {
            // TODO: delete extramenu button on UI ?

            InitializeComponent();

            RepositoriesController.Init();
            MainGitController.Init();

            MaximizedMinimizedWindow = new RelayCommand(headerControlBar_MouseLeftDoubleClick);
            headerBorder.InputBindings.Add(new InputBinding(MaximizedMinimizedWindow, new MouseGesture(MouseAction.LeftDoubleClick)));

            _animationController = new AnimationController();
            _textController = new TextController();
            _gitController = new GitController();
           
            _semaphore = new SemaphoreSlim(1);

            WeakReferenceMessenger.Default.Register<MainViewChangedMessage>(this, (r, m) =>
            { ResizeWindow(m); });

            StrongReferenceMessenger.Default.Register<UpdateGitHistoryMessage>(this, (r, m) =>
            { UpdateGitBarText(); });
        }

        /// <summary>
        /// Get GitCommandsHistory
        /// 25 commands, if bar is open, and 1 if bar is closed
        /// Format strings[] to Inlines using Runs
        /// (add colors, basically)
        /// Invoke Dispatcher, wait _semaphore
        /// (protection of messing up the commands order)
        /// (Clear Inlines, and add new Range)
        /// </summary>
        private void UpdateGitBarText()
        {
            Task.Run( async () =>
            {
                var count = _animationController.IsCommandsBarOpen ? 25 : 1;
                var text = await MainGitController.GetFormattedCommandsHistory(count);

                await GitCommandsTextBlock.Dispatcher.Invoke( async () => 
                {

                    await _semaphore.WaitAsync();
                    var lines = _textController.GetInlinesFromText(text);
                    GitCommandsTextBlock.Inlines.Clear();
                    GitCommandsTextBlock.Inlines.AddRange(lines);
                    _semaphore.Release();

                }, DispatcherPriority.Background);             
            });
        }



        /// <summary>
        /// Ask _animationController for Width and Height animations
        /// (if bar closed - opening animation, if open - closing animation
        /// Start animation
        /// Update bar text
        /// </summary>
        private void openCloseGitCommandsBar(object sender = null, EventArgs e = null)
        {
            var animations = _animationController.GetCommandsBarAnimation(GitCommandsButton.ActualHeight, GitCommandsButton.ActualWidth, false);

            GitCommandsButton.BeginAnimation(HeightProperty, animations.Height);
            GitCommandsButton.BeginAnimation(WidthProperty, animations.Width);

            UpdateGitBarText();
        }


        /// <summary>
        /// Invokes when view changes 
        /// Set new minimum Height and Height
        /// To restrict make window smaller than content
        /// </summary>
        private const int MarginHeight = 60;
        private const int MarginWidth = 30;
        private const int MenuMinHeight = 130;
        private void ResizeWindow(MainViewChangedMessage message)
        {
            var window = System.Windows.Application.Current.MainWindow;
            window.MinHeight = message.Value.MinHeight + MarginHeight;
            window.MinWidth = message.Value.MinWidth + MenuMinHeight + MarginWidth;
        }


        /// <summary>
        /// Internal Window OS command to drag window
        /// And implements default window behavior (split the screen etc.) 
        /// </summary>
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

        /// <summary>
        /// Maximize and Minimize the window on double click on header
        /// </summary>
        private void headerControlBar_MouseLeftDoubleClick()
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
            else
                this.WindowState = System.Windows.WindowState.Maximized;
        }

    }
}
