using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using GItClient.MVVM.Assets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        public const int ELLIPSE_SIZE = 8;

        private Dictionary<int, System.Windows.Media.Color> ColorByGeneration;

        public CommitsHistoryPartialView()
        {
            // TODO: ScrollViewer design

            InitializeComponent();

            RenderCommits();

            WeakReferenceMessenger.Default.Register<RepositoryChangedMessage>(this, (r, m) =>
            { RenderCommits(); });

            ColorByGeneration = new Dictionary<int, System.Windows.Media.Color> {
                { 0, Colors.White},
                { 1, Colors.Yellow},
                { 2, Colors.Green},
                { 3, Colors.Red},
                { 4, Colors.Blue},
                { 5, Colors.Violet}
            };
        }

        private void RenderCommits()
        {
            Dispatcher.Invoke( async () =>
            {
                MainGrid.RowDefinitions.Clear();

                MainCanvas.Children.Clear();
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
            MainGrid.SizeChanged -= GridChanged_ResizeSpinner;

            MainGrid.Children.Add(MainCanvas);
            Grid.SetRow(MainCanvas, 0);
            Grid.SetRowSpan(MainCanvas, int.MaxValue);
            Grid.SetColumn(MainCanvas, 0);

            var graphNodes = new Dictionary<string, TreeViewItem>();

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
            var commitsWithoutEmpty = currentRepository.CommitsHolder.Commits.Where(x => x.CommitHash != null).ToArray(); //TODO: temp
            var lastY = 0;
            for (var i = 0; i < commitsWithoutEmpty.Length; i++) 
            {
                var commit = commitsWithoutEmpty[i];

                if (commit.CommitHash == null)
                {
                    continue;
                }
                    
                var row = new RowDefinition();
                row.Height = new GridLength(rowHeight);

                var textblockBranch = CreateTextBlock(commit.Branch ?? "");
                var textblockHash = CreateTextBlock(commit.ShortCommitHash);
                var textblockMessage = CreateTextBlock(commit.Subject);
                var textblockAuthor = CreateTextBlock(commit.Author);
                var textblockDate = CreateTextBlock(commit.ShortDate.ToString("g"));

                MainGrid.RowDefinitions.Add(row);
                //MainGrid.Children.Add(textblockBranch);
                MainGrid.Children.Add(textblockHash);
                MainGrid.Children.Add(textblockMessage);
                MainGrid.Children.Add(textblockAuthor);
                MainGrid.Children.Add(textblockDate);
                // TOOD: MainGrid.Children.Add(graph);

                if (tree.AllNodes.ContainsKey(commit.CommitHash))
                {
                    var generation = tree.AllNodes[commit.CommitHash].Generation;
                    var indexInGeneration = tree.AllNodesByGeneration[generation].IndexOf(tree.AllNodes[commit.CommitHash]);

                    var body = new Ellipse() 
                    {
                        Height = 8,
                        Width = 8,
                        Fill = new SolidColorBrush(ColorByGeneration[indexInGeneration]),
                        ToolTip = new ToolTip() { Content = commit.Subject, Foreground = new SolidColorBrush(Colors.Black)},
                        HorizontalAlignment = HorizontalAlignment.Left,
                    };

                    var node = new TreeViewItem();
                    node.Body = body;
                    node.Commit = tree.AllNodes[commit.CommitHash].Data;

                    graphNodes[commit.CommitHash] = node;

                    //MainGrid.Children.Add(node.Body);
                    //Grid.SetRow(node.Body, i + 1);
                    //Grid.SetColumn(node.Body, 0);
                    MainCanvas.Children.Add(node.Body);
                    Canvas.SetTop(node.Body, lastY);
                    Canvas.SetLeft(node.Body, 10 * indexInGeneration + 5);

                    lastY += (int)rowHeight;

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

            

            DrawLines(graphNodes, out int maxWidth);

            MainGrid.ColumnDefinitions[0].Width = new GridLength(maxWidth);

        }
        public void DrawLines(Dictionary<string, TreeViewItem> allNodes, out int maxWidth)
        {
            var maxHeight = 0;
            maxWidth = 0;

            var maxParents = 1;
            foreach (var node in allNodes.Values)
            {
                if (node.Commit.ParentCommitHashes == null)
                {
                    continue;
                }

                for (var i = 0; i < node.Commit.ParentCommitHashes.Length; i++)
                {
                    if (i > maxParents) { maxParents = i; }

                    var parent = allNodes[node.Commit.ParentCommitHashes[i]];

                    var line = new Line();

                    line.Y1 = Canvas.GetTop(node.Body) + ELLIPSE_SIZE / 2;
                    line.X1 = Canvas.GetLeft(node.Body) + ELLIPSE_SIZE / 2;
                    line.Y2 = Canvas.GetTop(parent.Body) + ELLIPSE_SIZE / 2;
                    line.X2 = Canvas.GetLeft(parent.Body) + ELLIPSE_SIZE / 2;

                    line.Stroke = new SolidColorBrush(Colors.White);
                    line.StrokeThickness = 1;

                    maxHeight = (int)Math.Max(line.Y1, line.Y2);

                    MainCanvas.Children.Add(line);

                }
            }

            maxWidth = maxParents * (ELLIPSE_SIZE * 2) + 20;

            MainCanvas.Height = maxHeight + ELLIPSE_SIZE;

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

    public class TreeViewItem 
    {
        public Ellipse Body { get; set; }
        public GitCommitBase Commit { get; set; }

        public TreeViewItem() { }
    }

}
