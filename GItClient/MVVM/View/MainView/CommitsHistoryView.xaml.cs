using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GItClient.MVVM.View.MainView
{

    /// <summary>
    /// This is main view, 
    /// this class resposible for tabs
    /// All commits history is displayed in 
    /// CommitsHistoryPartialView
    /// </summary>
    public partial class CommitsHistoryView : UserControl
    {
        private UITab ActiveTab;
        private List<UITab> Tabs;

        public CommitsHistoryView()
        {
            Tabs = new List<UITab>();

            InitializeComponent();

            AddRepositoryTabs();
        }

        private void ChangeCurrentRepositoryAndUpdateUI(string repoName)
        {
            RepositoriesController.SetCurrentRepository(repoName);
            WeakReferenceMessenger.Default.Send(new RepositoryChangedMessage(repoName));
        }

        private void AddRepositoryTabs()
        {
            Tabs_Grid.ColumnDefinitions.Clear();
            Tabs_Grid.Children.Clear();

            var repositories = RepositoriesController.GetAllOpenRepositories();

            if (repositories.Length == 0)
            {
                AddRepositoryTab(null, 0);
                return;
            }

            for (var i = 0; i < repositories.Length; i++)
            {
                AddRepositoryTab(repositories[i], i);
            }
        }

        private void AddRepositoryTab(Repository? repository, int index)
        {
            var tab = new UITab();
            tab.Parent = Tabs_Grid;
            tab.AddTabToParent(index);

            Tabs.Add(tab);

            if (repository == null)
            {
                tab.ConvertToDefault();
                return;
            }

            tab.Name = repository.GenName;
            tab.Background = new SolidColorBrush(repository.Color);

            if (repository.Active)
            {
                tab.Activate();
                ActiveTab = tab;
            }

            tab.LeftButtonDown = button_Repository_Click;
            tab.CloseClick = button_Repository_Close;
        }

        private void button_Repository_Click(object sender, RoutedEventArgs e)
        {
            var tab = ((UITab)sender);

            if (ActiveTab == tab)
            {
                return;
            }

            ActiveTab.Deactivate();

            ActiveTab = tab;
            ActiveTab.Activate();

            var repoName = tab.Name;
            ChangeCurrentRepositoryAndUpdateUI(repoName);
        }


        private void button_Repository_Close(object sender, RoutedEventArgs e)
        {
            var tab = ((UITab)sender);
            var repoName = tab.Name;

            RepositoriesController.RemoveRepository(repoName);
            tab.Remove();
            Tabs.Remove(tab);

            if (ActiveTab == tab && Tabs.Count > 0)
            {
                var lastTab = Tabs.Last();
                lastTab.Activate();
                RepositoriesController.SetCurrentRepository(lastTab.Name);
                ActiveTab = lastTab;

                WeakReferenceMessenger.Default.Send(new RepositoryChangedMessage(""));
            }

            for (var i = 0; i < Tabs.Count; i++)
            {
                Tabs[i].SetColumn(i);
            }

            if (Tabs.Count == 0)
            {
                // Adding Welcome Tab, if no repository open
                AddRepositoryTabs();
                WeakReferenceMessenger.Default.Send(new RepositoryChangedMessage(""));
            }

            e.Handled = true;
        }


    }
}
