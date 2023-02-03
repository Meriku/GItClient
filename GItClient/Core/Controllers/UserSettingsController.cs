using GItClient.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers
{
    internal class UserSettingsController
    {
        private UserSettings? _userSettings;

        internal async Task SetAndSaveUserSettings(string? username, string? email, string? directory)
        {
            _userSettings = new UserSettings(username, email, directory);
            await SaveUserSettings();
        }

        internal UserSettings GetUserSettings()
        {
            if (_userSettings == null) UpdateUserSettings();
            return _userSettings; 
        }
        
        internal bool IsInitialSettingsFilled()
        {
            if (_userSettings == null) UpdateUserSettings();

            return _userSettings.Directory.Length > 0;
        }

        private void UpdateUserSettings()
        {
            _userSettings = SettingsController<UserSettings>.GetSpecificSetting();
        }

        private async Task SaveUserSettings()
        {
            if (_userSettings == null) { throw new ArgumentNullException(); }
            await SettingsController<UserSettings>.SetSpecificSetting(_userSettings);
        }

    }
}
