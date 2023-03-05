using GItClient.Core;
using GItClient.Core.Controllers;
using GItClient.Core.Controllers.SettingControllers;
using GItClient.Core.Convertors;
using GItClient.Core.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GItClient.MVVM.View.MainView
{
    /// <summary>
    /// Interaction logic for OpenRepoView.xaml
    /// </summary>
    public partial class OpenRepoView : UserControl
    {
        private UserSettings UserSettings;
        private UserSettingsController _userSettingsController;
        private GitController _gitController;
        private DirectoryController _directoryController;

        public OpenRepoView()
        {
            InitializeComponent();

            _userSettingsController = new UserSettingsController();
            _gitController = new GitController();
            _directoryController = new DirectoryController();

            UserSettings = _userSettingsController.GetUserSettings();

            User_Directory_Box.SizeChanged += TextBox_SizeChanged;
            User_Directory_Box.TextChanged += TextBox_SizeChanged;

            User_Directory_Box.Text = UserSettings.Directory;
        }

        private void TextBox_SizeChanged(object sender, RoutedEventArgs e)
        {
            User_Directory_Box.Text = TextTrimmer.TrimText((TextBox)sender, UserSettings.Directory);
        }

        private void onclick_Open_Directory_Dialog(object sender, MouseButtonEventArgs e)
        {
            var dialog = _directoryController.GetDirectoryDialog();

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                var textBox = ((TextBox)sender);
                UserSettings.Directory = dialog.FileName;
                textBox.Text = dialog.FileName;
            }
        }

        private void button_Open_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            button.IsEnabled = false;

            Task.Run(async () => 
            {
                var isRepoExist = await CheckIfExist();
                var isOpened = CheckIfOpen();

                if (isRepoExist && !isOpened) 
                {
                    _gitController.OpenRepository(UserSettings.Directory);
                }

                Dispatcher.Invoke(() => { button.IsEnabled = true; });
            });
        }

        private bool CheckIfOpen()
        {
            var genName = Helper.GetGeneratedNameFromPath(UserSettings.Directory);
            var isAlreadyOpen = RepositoriesController.IsRepositoryAdded(genName);

            if (isAlreadyOpen)
            {
                var messageBoxText = "This repository is already opened";
                var caption = "Error";
                var button = MessageBoxButton.OK;
                var icon = MessageBoxImage.Warning;

                var result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            }
            return isAlreadyOpen;
        }

        private async Task<bool> CheckIfExist()
        {
            var isExist = await _gitController.CheckIfRepositoryExist(UserSettings.Directory);

            if (!isExist)
            {
                var messageBoxText = "Folder does not contain any repository";
                var caption = "Error";
                var button = MessageBoxButton.OK;
                var icon = MessageBoxImage.Warning;

                var result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            }
            return isExist;
        }
    }
}
