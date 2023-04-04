using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Convertors;
using GItClient.Core.Models;
using GItClient.MVVM.Assets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
            currentRepository.RecalculateBranches();
            var tree = GitLogParser.CreateTree(currentRepository);
            var keys = tree.AllNodes.Keys.ToArray();
            

            //add rows
            for (var i = 0; i < keys.Length; i++)
            {
                var row = new RowDefinition() { Height = new GridLength(rowHeight) };
                MainGrid.RowDefinitions.Add(row);
            }

            var Height = MainGrid.RowDefinitions.Sum(x => x.ActualHeight);
            for (var i = 0; i < keys.Length; i++) 
            {
                var commit = tree.AllNodes[keys[i]].Data;

                var textblockHash = CreateTextBlock(commit.ShortCommitHash);
                var textblockMessage = CreateTextBlock(commit.Subject);
                var textblockAuthor = CreateTextBlock(commit.Author);
                var textblockDate = CreateTextBlock(commit.ShortDate.ToString("g"));

                MainGrid.Children.Add(textblockHash);
                MainGrid.Children.Add(textblockMessage);
                MainGrid.Children.Add(textblockAuthor);
                MainGrid.Children.Add(textblockDate);

                //ellipse           

                var body = new Ellipse()
                {
                    Height = ELLIPSE_SIZE,
                    Width = ELLIPSE_SIZE,
                    Fill = new SolidColorBrush(currentRepository.BranchesByName[commit.Branch].Color),
                    ToolTip = new ToolTip() { Content = commit.Branch, Foreground = new SolidColorBrush(Colors.Black) },
                    HorizontalAlignment = HorizontalAlignment.Left,
                    
                };

                var node = new TreeViewItem();
                node.Body = body;
                node.CommitNode = tree.AllNodes[keys[i]];

                graphNodes[node.CommitNode.Data.CommitHash] = node;
                MainCanvas.Children.Add(node.Body);

                var parent = tree.AllNodes[keys[i]].Parents.FirstOrDefault();
                if (parent == null)
                {
                    Canvas.SetLeft(node.Body, 10);
                }
                else
                {
                    var parentHash = parent.Data.CommitHash;
                    var parenyNode = graphNodes[parentHash];
                    var left = Canvas.GetLeft(parenyNode.Body);
                    if (parenyNode.CommitNode.Data.Branch == node.CommitNode.Data.Branch)
                    {
                        Canvas.SetLeft(node.Body, left);
                    }
                    else
                    {
                        Canvas.SetLeft(node.Body, left + 20);
                    }
                }
                Canvas.SetZIndex(node.Body, 10);

                Grid.SetRow(textblockHash, keys.Length - i);
                Grid.SetColumn(textblockHash, 1);

                Grid.SetRow(textblockMessage, keys.Length - i);
                Grid.SetColumn(textblockMessage, 2);

                Grid.SetRow(textblockAuthor, keys.Length - i);
                Grid.SetColumn(textblockAuthor, 3);

                Grid.SetRow(textblockDate, keys.Length - i);
                Grid.SetColumn(textblockDate, 4);
            }

            

            DrawLines(graphNodes, rowHeight, out int maxWidth);

            MainGrid.ColumnDefinitions[0].Width = new GridLength(maxWidth);

        }
        public void DrawLines(Dictionary<string, TreeViewItem> allNodes, double rowHeight, out int maxWidth)
        {
            var maxHeight = 0;
            maxWidth = 0;

            var Height = MainGrid.RowDefinitions[0].Height.Value + ELLIPSE_SIZE / 2;
            var maxChild = 1;
            foreach (var node in allNodes.Values.Reverse())
            {
                Canvas.SetTop(node.Body, Height);
                Height += rowHeight;
                for (var i = 0; i < node.CommitNode.Children.Count; i++)
                {
                    maxChild = Math.Max(maxChild, node.CommitNode.Children.Count);

                    var childHash = node.CommitNode.Children[i].Data.CommitHash;
                    var child = allNodes[childHash];

                    var line = new Line();

                    line.Y1 = Canvas.GetTop(node.Body) + ELLIPSE_SIZE / 2;
                    line.X1 = Canvas.GetLeft(node.Body) + ELLIPSE_SIZE / 2;
                    line.Y2 = Canvas.GetTop(child.Body) + ELLIPSE_SIZE / 2;
                    line.X2 = Canvas.GetLeft(child.Body) + ELLIPSE_SIZE / 2;

                    line.Stroke = new SolidColorBrush(Colors.White);
                    line.StrokeThickness = 1;

                    maxHeight = (int)Math.Max(line.Y1, line.Y2);

                    MainCanvas.Children.Add(line);
                }
            }

            maxWidth = maxChild * (20) + 40;

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
        public CommitsTreeNode CommitNode { get; set; }

        public TreeViewItem() { }
    }

}
