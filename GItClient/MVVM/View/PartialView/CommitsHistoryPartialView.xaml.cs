using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            // TODO: ScrollViewer design
            InitializeComponent();

            _repositoriesController = ControllersProvider.GetRepositoriesController();

            RenderCommits();

            WeakReferenceMessenger.Default.Register<RepositoryChangedMessage>(this, (r, m) =>
            { RenderCommits(); });
        }

        private void RenderCommits()
        {
            Dispatcher.Invoke(() =>
            {
                MainGrid.RowDefinitions.Clear();
                MainGrid.Children.Clear();

                if (_repositoriesController.IsEmpty)
                {
                    RenderWelcomeView();
                    return;
                }

                var CurrentRepository = _repositoriesController.GetCurrentRepository();
                if (CurrentRepository.CommitsHolder.IsEmpty)
                {
                    if (CurrentRepository.CommitsHolder.IsLoading)
                    {
                        RenderWaitingView(CurrentRepository);
                        return;
                    }
                    else
                    {
                        _repositoriesController.StartLoadRepositoryCommits(CurrentRepository.GenName);
                        RenderWaitingView(CurrentRepository);
                        return;
                    }
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
            textblock.Height = 50;
            textblock.HorizontalAlignment = HorizontalAlignment.Center;

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
            textblock.Height = 50;
            textblock.HorizontalAlignment = HorizontalAlignment.Center;

            MainGrid.RowDefinitions.Add(row);
            MainGrid.Children.Add(textblock);
            Grid.SetRow(textblock, 0);
            Grid.SetColumn(textblock, 0);

            Task.Run(async () => 
            {
                await currentRepository.CommitsHolder.WaitAsync();
                currentRepository.CommitsHolder.Release();
                WeakReferenceMessenger.Default.Send(new RepositoryChangedMessage("Wait"));
            });
        }

        private void RenderCommitsViewImpl(Repository currentRepository)
        {
            for (var i = 0; i < currentRepository.CommitsHolder.Lenght; i++) 
            {
                var commit = currentRepository.CommitsHolder.Commits[i];
                var row = new RowDefinition();
                row.Height = new GridLength(12);
    
                var textblockHash = new TextBlock();
                textblockHash.Text = commit.ShortCommitHash;
                textblockHash.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                textblockHash.FontSize = 10;

                var textblockMessage = new TextBlock();
                textblockMessage.Text = commit.CommitMessage;
                textblockMessage.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                textblockMessage.FontSize = 10;

                var textblockAuthor = new TextBlock();
                textblockAuthor.Text = commit.Author;
                textblockAuthor.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                textblockAuthor.FontSize = 10;

                MainGrid.RowDefinitions.Add(row);
                MainGrid.Children.Add(textblockHash);
                MainGrid.Children.Add(textblockMessage);
                MainGrid.Children.Add(textblockAuthor);
                // TOOD: MainGrid.Children.Add(graph);

                Grid.SetRow(textblockHash, i);
                Grid.SetColumn(textblockHash, 1);
                Grid.SetRow(textblockMessage, i);
                Grid.SetColumn(textblockMessage, 2);
                Grid.SetRow(textblockAuthor, i);
                Grid.SetColumn(textblockAuthor, 3);
            }

        }



    }
}
