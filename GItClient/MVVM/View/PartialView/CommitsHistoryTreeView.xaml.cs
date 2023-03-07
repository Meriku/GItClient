using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using Markdig.Extensions.Footnotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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
                var currentNode = tree.Head;
                var treeItem = new TreeViewItem() { Header = currentNode.Data.CommitHash };
                treeItem.IsExpanded = true;
                CommitsTree.Items.Add(treeItem);

                AddTreeItems(treeItem, currentNode);
            });
        }

        private void AddTreeItems(TreeViewItem parentNode, Node<GitCommitBase> currentNode)
        {
            foreach (var child in currentNode.Children)
            {
                var childItem = new TreeViewItem() { Header = child.Data.CommitHash };
                childItem.IsExpanded = true;
                parentNode.Items.Add(childItem);
                AddTreeItems(childItem, child);
            }
        }
    }
}
