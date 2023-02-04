using GItClient.Core.Base;
using GItClient.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers
{
    internal class UserSettingsController : SettingsBase<UserSettings>
    {
        private UserSettings? _userSettings;

        internal string GetDefaultDirectory()
        {
            return GetDefaultDrive() + "repos";
        }
        internal string GetDefaultDrive()
        {
            return DriveInfo.GetDrives()[0].Name;
        }
        internal async Task SetAndSaveUserSettings(string? username, string? email, string? directory)
        {
            _userSettings = new UserSettings(username, email, directory);
            await SaveUserSettings();
        }
        internal async Task SetAndSaveUserSettings(UserSettings userSettings)
        {
            _userSettings = userSettings;
            await SaveUserSettings();
        }
        internal UserSettings GetUserSettings()
        {
            UpdateUserSettings();
            return _userSettings.Clone();
        }
        internal bool IsInitialSettingsFilled()
        {
            UpdateUserSettings();
            return _userSettings.Directory.Length > 0;
        }
        private void UpdateUserSettings()
        {
            _userSettings ??= base.GetSpecificSetting();          
        }
        private async Task SaveUserSettings()
        {
            if (_userSettings == null) { throw new ArgumentNullException(); }
            await base.SetSpecificSetting(_userSettings);
        }
    }
}
