using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using System;
using System.Management.Automation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private Button ActiveButton;

        public CommitsHistoryView()
        {
            // TODO: add close buttons for tabs
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
            MainGrid.ColumnDefinitions.Clear();

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
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            var button = GenerateButton();

            if (repository == null)
            {
                button.Content = "Welcome!";
                button.Background = new SolidColorBrush(Color.FromArgb(255, 49, 43, 64));
            }
            else
            {

                button.Content = repository.GenName;
                button.Background = new SolidColorBrush(repository.Color);
                button.Background.Opacity = 0.6;

                if (repository.Active)
                { 
                    button.Background.Opacity = 1;
                    ActiveButton = button;
                }

                button.Click += button_Repository_Click;
                button.PreviewMouseRightButtonDown += button_Repository_Close;
            }

            MainGrid.Children.Add(button);
            Grid.SetRow(button, 0);
            Grid.SetColumn(button, index);
        }


        private void button_Repository_Click(object sender, RoutedEventArgs e)
        {
            var pressedButton = ((Button)sender);

            if (ActiveButton == pressedButton)
            {
                return;
            }

            ActiveButton.Background.Opacity = 0.6;

            ActiveButton = pressedButton;
            var repoName = ActiveButton.Content.ToString();
            ActiveButton.Background.Opacity = 1;

            ChangeCurrentRepositoryAndUpdateUI(repoName);
        }


        private void button_Repository_Close(object sender, RoutedEventArgs e)
        {
            //TODO: currently assign to right mouse click, to add button 
            var pressedButton = ((Button)sender);

            RepositoriesController.RemoveRepository(pressedButton.Content.ToString());

            AddRepositoryTabs();

            var activeRepo = RepositoriesController.GetCurrentRepository();
            ChangeCurrentRepositoryAndUpdateUI(activeRepo.GenName);
        }

        private Button GenerateButton()
        {
            var button = new Button();
            button.BorderThickness = new Thickness(0);
            button.Margin= new Thickness(0,0,5,0);
            return button;
        }

    }
}
