using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using System;
using System.Drawing;
using System.Management.Automation;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GItClient.MVVM.View.PartialView
{
    /// <summary>
    /// This is partial view of CommitsHistory
    /// When this view is loaded all commits are displayed
    /// </summary>
    public partial class CommitsHistoryPartialView : UserControl
    {
        private RepositoriesController _repositoriesController;

        public CommitsHistoryPartialView()
        {
            InitializeComponent();

            _repositoriesController = ControllersProvider.GetRepositoriesController();

            WeakReferenceMessenger.Default.Register<RepositoryChangedMessage>(this, (r, m) =>
            { RenderCommits(); });

            RenderCommits();
        }

        private void RenderCommits()
        {
            Dispatcher.Invoke(() =>
            {
                MainGrid.Children.Clear();

                if (_repositoriesController.IsEmpty)
                {
                    RenderWelcomeView();
                    return;
                }

                var CurrentRepository = _repositoriesController.GetCurrentRepository();
                if (CurrentRepository.CommitsHolder.IsEmpty)
                {
                    RenderWaitingView(CurrentRepository);
                    return;
                }

                RenderCommitsViewImpl(CurrentRepository);
            });
        }

        private void RenderWelcomeView()
        {
            var row = new RowDefinition();
            row.Height = new GridLength(60);

            var textblock = new TextBlock();

            textblock.Text = "It's curently empty here :( \nInit, clone or open a new repository";
            textblock.Margin = new Thickness(10);
            textblock.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            textblock.FontSize = 15;

            MainGrid.RowDefinitions.Add(row);
            MainGrid.Children.Add(textblock);
            Grid.SetRow(textblock, 0);
            Grid.SetColumn(textblock, 0);
        }

        private void RenderWaitingView(Repository currentRepository)
        {
            var row = new RowDefinition();
            row.Height = new GridLength(60);

            var textblock = new TextBlock();

            textblock.Text = "Waiting";
            textblock.Margin = new Thickness(10);
            textblock.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            textblock.FontSize = 15;

            MainGrid.RowDefinitions.Add(row);
            MainGrid.Children.Add(textblock);
            Grid.SetRow(textblock, 0);
            Grid.SetColumn(textblock, 0);

            Task.Run(async () => 
            {
                await currentRepository.CommitsHolder.semaphore.WaitAsync();
                WeakReferenceMessenger.Default.Send(new RepositoryChangedMessage(new Repository()));
                currentRepository.CommitsHolder.semaphore.Release();
            });
        }

        private void RenderCommitsViewImpl(Repository currentRepository)
        {
            //TODO: add cache

            for (var i = 0; i < currentRepository.CommitsHolder.Lenght; i++) 
            {
                var commit = currentRepository.CommitsHolder.Commits[i];
                var row = new RowDefinition();
                row.Height = new GridLength(30);
    
                var textblock = new TextBlock();

                textblock.Text = commit.ShortCommitHash + " " + commit.CommitMessage + " " + commit.Author;
                textblock.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                textblock.FontSize = 10;
                textblock.Height = 12;

                var border = new Border();
                border.Width = this.Width - 20;
                border.Height = 30;
                border.Child = textblock;
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));

                MainGrid.RowDefinitions.Add(row);
                MainGrid.Children.Add(border);
                Grid.SetRow(border, i);
                Grid.SetColumn(border, 0);
            }

        }



    }
}
