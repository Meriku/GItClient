using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using GItClient.MVVM.Assets;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            Dispatcher.Invoke(() =>
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


                RenderCommitsViewImpl(CurrentRepository);
            });
        }

        private void RenderWelcomeView()
        {
            var row = new RowDefinition();
            row.Height = new GridLength(60);

            var textblock = new TextBlock();

            textblock.Text = EMPTY_REPOSITORIES_TEXT;
            textblock.Margin = new Thickness(10);
            textblock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            textblock.FontSize = 15;
            textblock.Height = 50;
            textblock.HorizontalAlignment = HorizontalAlignment.Center;

            MainGrid.RowDefinitions.Add(row);
            MainGrid.Children.Add(textblock);
            Grid.SetRow(textblock, 0);
            Grid.SetColumn(textblock, 2);
        }

        private void RenderWaitingView(Repository currentRepository)
        {
            var spinner = new LoadingSpinner();

            spinner.Name = "Spinner";
            spinner.Color = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            spinner.Diameter = MainGrid.ActualHeight < MainGrid.ActualWidth ? MainGrid.ActualHeight / 2 : MainGrid.ActualWidth / 2;
            spinner.Thickness = 5;
            spinner.IsLoading = true;
            spinner.Visibility = Visibility.Visible;

            MainGrid.SizeChanged += GridChanged_ResizeSpinner;

            MainGrid.Children.Add(spinner);
            Grid.SetRow(spinner, 0);
            Grid.SetColumn(spinner, 0);
            Grid.SetColumnSpan(spinner, 4);


            Task.Run(async () => 
            {
                await currentRepository.CommitsHolder.WaitAsync();
                currentRepository.CommitsHolder.Release();

                WeakReferenceMessenger.Default.Send(new RepositoryChangedMessage("WaitingEnded"));
            });
        }

        private void RenderCommitsViewImpl(Repository currentRepository)
        {
            var fontSize = FontSize;
            var fontFamily = new FontFamily("Roboto-Light");
            var fontColor = new SolidColorBrush(Color.FromRgb(190, 190, 190));

            var rowHeight = Math.Ceiling(fontSize * fontFamily.LineSpacing);
        
            var EmptyFirstRow = new RowDefinition();
            EmptyFirstRow.Height = new GridLength(5);
            MainGrid.RowDefinitions.Add(EmptyFirstRow);

            for (var i = 0; i < currentRepository.CommitsHolder.Lenght; i++) 
            {
                MainGrid.SizeChanged -= GridChanged_ResizeSpinner;

                var commit = currentRepository.CommitsHolder.Commits[i];
                var row = new RowDefinition();
                row.Height = new GridLength(rowHeight);
    
                var textblockHash = new TextBlock();
                textblockHash.Text = commit.ShortCommitHash;
                textblockHash.Foreground = fontColor;
                textblockHash.FontSize = fontSize;
                textblockHash.FontFamily = fontFamily;

                var textblockMessage = new TextBlock();
                textblockMessage.Text = commit.CommitMessage;
                textblockMessage.Foreground = fontColor;
                textblockMessage.FontSize = fontSize;
                textblockMessage.FontFamily = fontFamily;

                var textblockAuthor = new TextBlock();
                textblockAuthor.Text = commit.Author;
                textblockAuthor.Foreground = fontColor;
                textblockAuthor.FontSize = fontSize;
                textblockAuthor.FontFamily = fontFamily;

                MainGrid.RowDefinitions.Add(row);
                MainGrid.Children.Add(textblockHash);
                MainGrid.Children.Add(textblockMessage);
                MainGrid.Children.Add(textblockAuthor);
                // TOOD: MainGrid.Children.Add(graph);

                Grid.SetRow(textblockHash, i + 1);
                Grid.SetColumn(textblockHash, 1);

                Grid.SetRow(textblockMessage, i + 1);
                Grid.SetColumn(textblockMessage, 2);

                Grid.SetRow(textblockAuthor, i + 1);
                Grid.SetColumn(textblockAuthor, 3);
            }

        }

        private void ResizeGridText()
        {
            var fontSize = FontSize;
            var fontFamily = new FontFamily("Roboto-Light");
            var rowHeight = Math.Ceiling(fontSize * fontFamily.LineSpacing);

            for (var i = 1; i < MainGrid.RowDefinitions.Count; i++)
            {
                MainGrid.RowDefinitions[i].Height = new GridLength(rowHeight);
            }
            foreach (var child in MainGrid.Children)
            {
                if (child is TextBlock textBlock)
                {
                    textBlock.FontSize = fontSize;
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

    }
}
