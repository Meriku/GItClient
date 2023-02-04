using GItClient.Core;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
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

namespace GItClient.MVVM.View.MainView
{
    /// <summary>
    /// Interaction logic for InitRepoView.xaml
    /// </summary>
    public partial class InitRepoView : UserControl
    {
        private UserSettings UserSettings;
        private UserSettingsController _userSettingsController;
        private GitController _gitController;

        public InitRepoView()
        {
            InitializeComponent();

            _userSettingsController = ControllersProvider.GetUserSettingsController();
            _gitController = ControllersProvider.GetGitController();
            UserSettings = _userSettingsController.GetUserSettings();

            User_Directory_Box.Text = Helper.TrimDirectoryName(UserSettings.Directory);
        }

        private void onclick_Open_Directory_Dialog(object sender, MouseButtonEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                UserSettings.Directory = dialog.FileName;
                ((TextBox)sender).Text = Helper.TrimDirectoryName(dialog.FileName);
            }
        }

        private void button_Create_Click(object sender, RoutedEventArgs e)
        {
            _gitController.InitRepository(UserSettings.Directory);
        }
    }
}
