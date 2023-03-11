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
                var head = tree.Head;
                
                for (var i = 0; ; i++)
                {
                    MainGrid.RowDefinitions.Add(new RowDefinition());

                    if (!tree.AllNodesByGeneration.ContainsKey(i))
                    {
                        break;
                    }

                    var nodes = tree.AllNodesByGeneration[i];

                    for (var n = 0; n < nodes.Count; n++)
                    {
                        if (MainGrid.ColumnDefinitions.Count <= n)
                        {
                            MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        }

                        var viewNode = new TreeViewItem<GitCommitBase>(nodes[n]);

                        MainGrid.Children.Add(viewNode.Body);
                        Grid.SetRow(viewNode.Body, i);
                        Grid.SetColumn(viewNode.Body, n);

                    }
                    



                }

            });
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
        Body.Height = 20;
        Body.Width = 20;
        Body.Fill = new SolidColorBrush(Colors.White);

        Body.ToolTip = new ToolTip() { Content = Data.GetHash(), Foreground = new SolidColorBrush(Colors.Black) };     
    }

    public string GetHash()
    {
        return Data.Data.GetHash();
    }
}