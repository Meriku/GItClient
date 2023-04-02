using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using GItClient.MVVM.Assets;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GItClient.MVVM.View.PartialView
{
    /// <summary>
    /// This is partial view of CommitsHistory
    /// When this view is loaded all commits are displayed
    /// </summary>
    public partial class CommitsHistoryPartialView : UserControl
    {
        private const string EMPTY_REPOSITORIES_TEXT = "It's curently empty here :( \nInit, clone or open a new repository";

        private int FontSize = 12;

        public CommitsHistoryPartialView()
        {
            // TODO: ScrollViewer design

            InitializeComponent();

            RenderCommits();

            WeakReferenceMessenger.Default.Register<RepositoryChangedMessage>(this, (r, m) =>
            { RenderCommits(); });
        }

        private void RenderCommits()
        {
            Dispatcher.Invoke( async () =>
            {
                MainGrid.RowDefinitions.Clear();
                MainGrid.Children.Clear();

                if (RepositoriesController.IsEmpty)
                {
                    RenderWelcomeView();
                    return;
                }

                var CurrentRepository = RepositoriesController.GetCurrentRepository();
                if (CurrentRepository.CommitsHolder.IsEmpty)
                {
                    if (CurrentRepository.CommitsHolder.IsLoading)
                    {
                        RenderWaitingView(CurrentRepository);
                        return;
                    }
                    else
                    {
                        RepositoriesController.StartLoadRepositoryCommits(CurrentRepository.GenName);
                        RenderWaitingView(CurrentRepository);
                        return;
                    }
                }


                await RenderCommitsViewImpl(CurrentRepository);
            });
        }

        private void RenderWelcomeView()
        {
            var row = new RowDefinition();
            row.Height = new GridLength(60);

            var textblock = new TextBlock();

            textblock.Text = EMPTY_REPOSITORIES_TEXT;
            textblock.Margin = new Thickness(10);
            textblock.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            textblock.FontSize = 15;
            textblock.Height = 50;
            textblock.HorizontalAlignment = HorizontalAlignment.Center;

            MainGrid.RowDefinitions.Add(row);
            MainGrid.Children.Add(textblock);
            Grid.SetRow(textblock, 1);
            Grid.SetColumn(textblock, 2);
        }

        private void RenderWaitingView(Repository currentRepository)
        {
            var spinner = new LoadingSpinner();

            spinner.Name = "Spinner";
            spinner.Color = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            spinner.Diameter = MainGrid.ActualHeight < MainGrid.ActualWidth ? MainGrid.ActualHeight / 2 : MainGrid.ActualWidth / 2;
            spinner.Thickness = 5;
            spinner.IsLoading = true;
            spinner.Visibility = Visibility.Visible;

            MainGrid.SizeChanged += GridChanged_ResizeSpinner;

            MainGrid.Children.Add(spinner);
            Grid.SetRow(spinner, 1);
            Grid.SetColumn(spinner, 0);
            Grid.SetColumnSpan(spinner, 4);


            Task.Run(async () => 
            {
                await currentRepository.CommitsHolder.WaitAsync();
                currentRepository.CommitsHolder.Release();

                WeakReferenceMessenger.Default.Send(new RepositoryChangedMessage("WaitingEnded"));
            });
        }

        private async Task RenderCommitsViewImpl(Repository currentRepository)
        {
            var fontSize = FontSize;
            var fontFamily = new System.Windows.Media.FontFamily("Roboto-Light");
            var fontColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));

            var rowHeight = GetRowHeight();

            var EmptyFirstRow = new RowDefinition();
            EmptyFirstRow.Height = new GridLength(5);
            MainGrid.RowDefinitions.Add(EmptyFirstRow);

            //tree
            var gitController = new GitController();
            var tree = await gitController.GetGitCommitsTreeAsync(currentRepository);

            for (var i = 0; i < currentRepository.CommitsHolder.Lenght; i++) 
            {
                MainGrid.SizeChanged -= GridChanged_ResizeSpinner;

                var commit = currentRepository.CommitsHolder.Commits[i];
                var row = new RowDefinition();
                row.Height = new GridLength(rowHeight);

                var textblockBranch = CreateTextBlock(commit.Branch ?? "");
                var textblockHash = CreateTextBlock(commit.ShortCommitHash);
                var textblockMessage = CreateTextBlock(commit.Subject);
                var textblockAuthor = CreateTextBlock(commit.Author);
                var textblockDate = CreateTextBlock(commit.ShortDate.ToString("g"));

                MainGrid.RowDefinitions.Add(row);
                MainGrid.Children.Add(textblockBranch);
                MainGrid.Children.Add(textblockHash);
                MainGrid.Children.Add(textblockMessage);
                MainGrid.Children.Add(textblockAuthor);
                MainGrid.Children.Add(textblockDate);
                // TOOD: MainGrid.Children.Add(graph);
                if (tree.AllNodes.ContainsKey(commit.CommitHash))
                {
                    var node = new Ellipse() 
                    {
                        Height = 8,
                        Width = 8,
                        Fill = new SolidColorBrush(Colors.Wheat),
                        ToolTip = new ToolTip() { Content = commit.Subject, Foreground = new SolidColorBrush(Colors.Black)}
                    };

                    MainGrid.Children.Add(node);
                    Grid.SetRow(node, i + 1);
                    Grid.SetColumn(node, 0);

                }

                //Grid.SetRow(textblockBranch, i + 1);
                //Grid.SetColumn(textblockBranch, 0);

                Grid.SetRow(textblockHash, i + 1);
                Grid.SetColumn(textblockHash, 1);

                Grid.SetRow(textblockMessage, i + 1);
                Grid.SetColumn(textblockMessage, 2);

                Grid.SetRow(textblockAuthor, i + 1);
                Grid.SetColumn(textblockAuthor, 3);

                Grid.SetRow(textblockDate, i + 1);
                Grid.SetColumn(textblockDate, 4);
            }


        }

        
        private TextBlock CreateTextBlock(string text)
        {
            var fontSize = FontSize;
            var fontFamily = new System.Windows.Media.FontFamily("Roboto-Light");
            var fontColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));

            var textblock = new TextBlock();
            textblock.Foreground = fontColor;
            textblock.FontSize = fontSize;
            textblock.FontFamily = fontFamily;
            textblock.Text = text;

            return textblock;
        }
        private void ResizeGridText()
        {
            var rowHeight = GetRowHeight();

            for (var i = 1; i < MainGrid.RowDefinitions.Count; i++)
            {
                MainGrid.RowDefinitions[i].Height = new GridLength(rowHeight);
            }
            foreach (var child in MainGrid.Children)
            {
                if (child is TextBlock textBlock)
                {
                    textBlock.FontSize = FontSize;
                }
            }
        }

        private void GridChanged_ResizeSpinner(object o, SizeChangedEventArgs e)
        {
            var gridChild = MainGrid.Children[0];

            if (gridChild != null && gridChild is LoadingSpinner spinner)
            {
                spinner.Diameter = MainGrid.ActualHeight < MainGrid.ActualWidth ? MainGrid.ActualHeight / 2 : MainGrid.ActualWidth / 2;
            }
        }

        private void PreviewMouseWheel_ResizeCommitsFont(object sender, MouseWheelEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                return;
            }

            if (e.Delta > 0 && FontSize < 30)
            {
                FontSize++;
                ResizeGridText();
            }
            else if (e.Delta < 0 && FontSize > 8)
            {
                FontSize--;
                ResizeGridText();
            }

        }

        private double GetRowHeight()
        {
            var fontSize = FontSize;
            var fontFamily = new System.Windows.Media.FontFamily("Roboto-Light");
            var rowHeight = Math.Ceiling(fontSize * fontFamily.LineSpacing) + 5;
            return rowHeight;
        }

    }
}
