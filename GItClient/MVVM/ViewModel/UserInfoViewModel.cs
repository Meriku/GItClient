using GItClient.Core.Controllers;
using GItClient.Core.Models;
using GItClient.MVVM.View.MainView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GItClient.MVVM.ViewModel
{
    class UserInfoViewModel : IViewModel
    {
        private UserSettings UserSettings;

        public string Username { get; set; }
        public string Email { get; set; }
        public string Directory { get; set; }

        public double MinWidth { get => 550; }
        public double MinHeight { get => 220; }

        public UserInfoViewModel()
        {
            UserSettings = SettingsController<UserSettings>.GetSpecificSetting();

            Username = UserSettings.Username;
            Email = UserSettings.Email;
            Directory = UserSettings.Directory;
        }
    }
}
