using GItClient.Core;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using GItClient.MVVM.View.MainView;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GItClient.MVVM.ViewModel
{
    class UserInfoViewModel : IViewModel
    {
        private UserSettings UserSettings;
        private UserSettingsController _userSettingsController;

        public string Username { get; set; }
        public string Email { get; set; }
        public string Directory { get; set; }

        public double MinWidth { get => 550; }
        public double MinHeight { get => 220; }

        public UserInfoViewModel()
        {
            _userSettingsController = ControllersProvider.GetUserSettingsController();
            UserSettings = _userSettingsController.GetUserSettings();

            Username = UserSettings.Username;
            Email = UserSettings.Email;
            Directory = Helper.TrimDirectoryName(UserSettings.Directory);
        }
    }
}
