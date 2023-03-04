using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using GItClient.MVVM.Assets;
using System;
using System.Management.Automation;
using System.Windows;
using System.Windows.Automation.Peers;
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
        private Border ActiveBorder;

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
            Tabs_Grid.ColumnDefinitions.Clear();

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
            var column = new ColumnDefinition();
            Tabs_Grid.ColumnDefinitions.Add(column);

            var border = new Border();
            var grid = new Grid();
            var label = new TextBlock();
            var button = new CrossButton();


            button.Width = 13;
            button.Height = 13;
            button.HorizontalAlignment = HorizontalAlignment.Right;
            button.Margin = new Thickness(0,0,6,0);

            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            border.Child = grid;

            grid.Children.Add(label);
            grid.Children.Add(button);
            Grid.SetRow(label, 0);
            Grid.SetColumn(label, 0);
            Grid.SetColumnSpan(label, 2);
            Grid.SetRow(button, 0);
            Grid.SetColumn(button, 1);

            if (repository == null)
            {
                border.Name = "Welcome!";
                label.Text = "Welcome!";
                border.Background = new SolidColorBrush(Color.FromArgb(255, 49, 43, 64));
            }
            else
            {
                border.Name = repository.GenName;
                label.Text = repository.GenName;
                border.Background = new SolidColorBrush(repository.Color);
                border.Background.Opacity = 0.6;

                if (repository.Active)
                {
                    border.Background.Opacity = 1;
                    ActiveBorder = border;
                }

                border.PreviewMouseLeftButtonDown += button_Repository_Click;
                border.PreviewMouseRightButtonDown += button_Repository_Close;
            }

            Tabs_Grid.Children.Add(border);
            Grid.SetRow(border, 0);
            Grid.SetColumn(border, index);
        }


        private void button_Repository_Click(object sender, RoutedEventArgs e)
        {
            var pressedBorder = ((Border)sender);

            if (ActiveBorder == pressedBorder)
            {
                return;
            }

            ActiveBorder.Background.Opacity = 0.6;

            ActiveBorder = pressedBorder;
            var repoName = ActiveBorder.Name;
            ActiveBorder.Background.Opacity = 1;

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
