using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using Microsoft.WindowsAPICodePack.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using Task = System.Threading.Tasks.Task;

namespace GItClient.MVVM.View.PartialView
{
    /// <summary>
    /// Interaction logic for CommitsHistoryTreeView.xaml
    /// </summary>
    public partial class CommitsHistoryTreeView : UserControl
    {

        public const int ELLIPSE_SIZE = 8;

        private Dictionary<string, GitCommit> CurrentRepositoryCommits;
        public CommitsHistoryTreeView()
        {
            InitializeComponent();

            LoadCommitsTree();

            WeakReferenceMessenger.Default.Register<RepositoryChangedMessage>(this, (r, m) =>
            { LoadCommitsTree(); });
        }

        public void LoadCommitsTree()
        {
            Task.Run(async () => 
            { 
                var currentRepository = RepositoriesController.GetCurrentRepository();
                var gitController = new GitController();
                //TODO: temp
                CurrentRepositoryCommits = currentRepository.CommitsHolder.Commits.Where(x => x.CommitHash != null).ToDictionary(x => x.CommitHash, x => x);
                var tree = await gitController.GetGitCommitsTreeAsync(currentRepository);
                RenderCommitsTree(tree);
            });


        }
        public void RenderCommitsTree(Tree<GitCommitBase> tree)
        {
    
            Dispatcher.Invoke(() =>
            {
                var AllNodes = new Dictionary<string, TreeViewItem<GitCommitBase>>();

                var lastY = 0;

                for (var i = tree.AllNodesByGeneration.Count - 1; ; i--)
                {
                    
                    if (!tree.AllNodesByGeneration.ContainsKey(i))
                    {            
                        break;
                    }

                   
                    var nodes = tree.AllNodesByGeneration[i];

                    for (var n = 0; n < nodes.Count; n++)
                    {
                        //horizontal position
                        var x = n * ELLIPSE_SIZE * 2;

                        var hash = nodes[n].GetHash();
                        TreeViewItem<GitCommitBase> viewNode = null;

                        if (CurrentRepositoryCommits.ContainsKey(hash))
                        {
                            var commit = CurrentRepositoryCommits[hash];
                            var message = commit.Subject + " " + commit.Date + " " + commit.Author;
                            viewNode = new TreeViewItem<GitCommitBase>(nodes[n], message, ELLIPSE_SIZE, Colors.White);
                        }
                        else
                        {
                            viewNode = new TreeViewItem<GitCommitBase>(nodes[n], hash, ELLIPSE_SIZE, Colors.Red);
                        }

                                      

                        MainCanvas.Children.Add(viewNode.Body);
                        Canvas.SetTop(viewNode.Body, lastY);
                        Canvas.SetLeft(viewNode.Body, x);

                        AllNodes[viewNode.GetHash()] = viewNode;

                        //vertical position
                        lastY += ELLIPSE_SIZE * 2;
                    }
                }

                MainCanvas.Height = lastY;

                DrawLines(AllNodes);


            });
        }

        public void DrawLines(Dictionary<string, TreeViewItem<GitCommitBase>> allNodes)
        {
            foreach (var node in allNodes.Values)
            {
                var childHashes = node.Data.Children.Select(x => x.GetHash()).ToArray();

                for (var i = 0; i < childHashes.Length; i++)
                {
                    var childNode = allNodes[childHashes[i]];

                    var line = new Line();

                    line.Y1 = Canvas.GetTop(node.Body) + ELLIPSE_SIZE / 2;
                    line.X1 = Canvas.GetLeft(node.Body) + ELLIPSE_SIZE / 2;
                    line.Y2 = Canvas.GetTop(childNode.Body) + ELLIPSE_SIZE / 2;
                    line.X2 = Canvas.GetLeft(childNode.Body) + ELLIPSE_SIZE / 2;

                    line.Stroke = new SolidColorBrush(Colors.White);
                    line.StrokeThickness = 1;

                    MainCanvas.Children.Add(line);

                }
            }

        }

    }
}

public class TreeView<T> where T : IGetHash, IGetParentHashes
{
    public TreeViewItem<T> Head { get; set; }

    public Dictionary<string, TreeViewItem<T>> AllNodes { get; set; }
    public TreeView()
    {
        AllNodes = new Dictionary<string, TreeViewItem<T>>();
    }
}

public class TreeViewItem<T> where T : IGetHash, IGetParentHashes
{
    public Line[] Edges { get; set; }
    public Ellipse Body { get; set; }
    public TreeNode<T> Data { get; set; }
    public Point Location { get; set; }

    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }

    public TreeViewItem(TreeNode<T> data, string message, int size, Color color)
    {
        Data = data;

        Body = new Ellipse();
        Body.Height = size;
        Body.Width = size;
        Body.Fill = new SolidColorBrush(color);

        Body.ToolTip = new ToolTip() { Content = message, Foreground = new SolidColorBrush(Colors.Black) };     
    }

    public string GetHash()
    {
        return Data.Data.GetHash();
    }
}