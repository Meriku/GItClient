using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using GItClient.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
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
        private GitController _gitController;
        private RepositoriesController _repositoriesController;

        private int ActiveRepos = 0;

        public CommitsHistoryView()
        {
            // TODO: add close buttons for tabs
            InitializeComponent();

            _gitController = ControllersProvider.GetGitController();
            _repositoriesController = ControllersProvider.GetRepositoriesController();

            AddRepositoryTabs();

        }

        private void button_Repository_Click(object sender, RoutedEventArgs e)
        {
            var repoName = ((Button)sender).Content.ToString();
            var repo = _repositoriesController.GetSpecificRepository(repoName);
            WeakReferenceMessenger.Default.Send(new RepositoryChangedMessage(repo));

        }


        private void AddRepositoryTabs()
        {
            var repositories = _repositoriesController.GetAllOpenRepositories();

            if (repositories.Length == 0)
            {
                AddWelcomeRepositoryTab();
                return;
            }

            foreach (var repository in repositories)
            {
                AddRepositoryTab(repository);
            }

        }

        private void AddRepositoryTab(Repository repository)
        {
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            Random random = new Random();
            var color = Color.FromRgb((byte)random.Next(100, 200), (byte)random.Next(100, 200), (byte)random.Next(100, 200));

            var button = new Button();
            button.Background = new SolidColorBrush(color);
            button.Content = repository.GenName;

            button.Click += button_Repository_Click;

            MainGrid.Children.Add(button);
            Grid.SetRow(button, 0);
            Grid.SetColumn(button, ActiveRepos);

            ActiveRepos++;
        }


        private void AddWelcomeRepositoryTab()
        {
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            Random random = new Random();
            var color = Color.FromRgb((byte)random.Next(100, 200), (byte)random.Next(100, 200), (byte)random.Next(100, 200));

            var button = new Button();
            button.Background = new SolidColorBrush(color);
            button.Content = "Welcome!";

            MainGrid.Children.Add(button);
            Grid.SetRow(button, 0);
            Grid.SetColumn(button, ActiveRepos);

            //TODO: do not need to send repo
            WeakReferenceMessenger.Default.Send(new RepositoryChangedMessage(new Repository()));
        }


        private void Button_RemoveRepos_Click(object sender, MouseButtonEventArgs e)
        {
            // TODO: interaction logic when press close button on the tab

            //if (MainGrid.ColumnDefinitions.Any())
            //{
            //    MainGrid.ColumnDefinitions.Remove(MainGrid.ColumnDefinitions.Last());
            //    ActiveRepos--;
            //}
        }

    }
}
