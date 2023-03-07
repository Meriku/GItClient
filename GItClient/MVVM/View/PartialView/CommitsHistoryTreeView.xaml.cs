using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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

                var head = tree.Head;
                var treeHead = new TreeViewItem<GitCommitBase>(head, MainCanvas);
                //Tree with all children are created here

                var X = (int)MainCanvas.ActualWidth / 2;
                var Y = 0;

                Canvas.SetLeft(treeHead.Body, X);
                Canvas.SetTop(treeHead.Body, Y);

                MainCanvas.Height = 3000;

                SetTreeItemsPosition(treeHead, X, Y);
            });
        }

        private void SetTreeItemsPosition(TreeViewItem<GitCommitBase> treeNode, int x, int y)
        {
            var X = x;
            var Y = y;

            Y += 30;
            X -= treeNode.Children.Length * 20;

            foreach (var child in treeNode.Children)
            {
                Canvas.SetLeft(child.Body, X);
                Canvas.SetTop(child.Body, Y);

                X += 20;

                SetTreeItemsPosition(child, X, Y);
            }
        }
    }
}

public class TreeView<T> where T : IGetHash, IGetParentHashes
{
    public TreeViewItem<T> Head { get; set; }

    public TreeView()
    {

    }
}

public class TreeViewItem<T> where T : IGetHash, IGetParentHashes
{
    public TreeViewItem<T>[] Children { get; set; }
    public Line[] Edges { get; set; }
    public Ellipse Body { get; set; }
    public TreeNode<T> Data { get; set; }

    public TreeViewItem(TreeNode<T> data, Canvas canvas)
    {
        Data = data;

        Body = new Ellipse();
        Body.Height = 20;
        Body.Width = 20;
        Body.Fill = new SolidColorBrush(Colors.White);

        canvas.Children.Add(Body);

        var children = new List<TreeViewItem<T>>(Data.Children.Count);
        foreach (var child in Data.Children)
        {
            children.Add(new TreeViewItem<T>(child, canvas));
        }
        Children = children.ToArray();

    }
}