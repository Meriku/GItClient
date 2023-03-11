using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
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
                var tree = await gitController.GetGitCommitsTreeAsync(currentRepository);
                RenderCommitsTree(tree);
            });


        }
        public void RenderCommitsTree(Tree<GitCommitBase> tree)
        {
    
            Dispatcher.Invoke(() =>
            {
                var AllNodes = new Dictionary<string, TreeViewItem<GitCommitBase>>();

                for (var i = 0; ; i++)
                {
                    
                    if (!tree.AllNodesByGeneration.ContainsKey(i))
                    {            
                        break;
                    }

                    MainCanvas.Height = i * 30;

                    var nodes = tree.AllNodesByGeneration[i];

                    for (var n = 0; n < nodes.Count; n++)
                    {
                        var y = i * 30;
                        var x = n * 30;

                        var viewNode = new TreeViewItem<GitCommitBase>(nodes[n]);

                        MainCanvas.Children.Add(viewNode.Body);
                        Canvas.SetTop(viewNode.Body, y);
                        Canvas.SetLeft(viewNode.Body, x);

                        AllNodes[viewNode.GetHash()] = viewNode;
                    }
                }

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

                    line.Y1 = Canvas.GetTop(node.Body) + 8;
                    line.X1 = Canvas.GetLeft(node.Body) + 8;
                    line.Y2 = Canvas.GetTop(childNode.Body) + 8;
                    line.X2 = Canvas.GetLeft(childNode.Body) + 8;

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

    public TreeViewItem(TreeNode<T> data)
    {
        Data = data;

        Body = new Ellipse();
        Body.Height = 16;
        Body.Width = 16;
        Body.Fill = new SolidColorBrush(Colors.White);

        Body.ToolTip = new ToolTip() { Content = Data.GetHash(), Foreground = new SolidColorBrush(Colors.Black) };     
    }

    public string GetHash()
    {
        return Data.Data.GetHash();
    }
}