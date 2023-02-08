﻿using GItClient.Core;
using GItClient.Core.Controllers;
using GItClient.Core.Convertors;
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
    /// Interaction logic for CloneRepoView.xaml
    /// </summary>
    public partial class CloneRepoView : UserControl
    {
        private UserSettings UserSettings;
        private UserSettingsController _userSettingsController;
        private GitController _gitController;
        private DirectoryController _directoryController;

        private string Link;

        public CloneRepoView()
        {
            InitializeComponent();

            _userSettingsController = ControllersProvider.GetUserSettingsController();
            _gitController = ControllersProvider.GetGitController();
            _directoryController = ControllersProvider.GetDirectoryController();

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

        private void Link_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Helper.IsValidLink(User_Link_Box.Text))
            {
                Link = User_Link_Box.Text;
                link_Error_Icon.Visibility = Visibility.Hidden;
                button_Clone.IsEnabled = true;
            }
            else
            {
                Link = "";
                link_Error_Icon.Visibility = Visibility.Visible;
                button_Clone.IsEnabled = false;
            }
            
        }

        private void button_Clone_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Link))
            {
                _gitController.CloneRepository(UserSettings.Directory, Link);
            }
        }
    }
}