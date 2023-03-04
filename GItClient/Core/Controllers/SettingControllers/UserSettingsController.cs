using GItClient.Core.Base;
using GItClient.Core.Controllers.Static;
using GItClient.Core.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers.SettingControllers
{
    /// <summary>
    /// Controller
    /// Easy way to access UserSettings
    /// Model: public class UserSettings : ISetting
    /// </summary>
    internal class UserSettingsController : SettingsBase<UserSettings>
    {
        private UserSettings? _userSettings;
        private UserSettings UserSettings
        {
            get
            {
                if (_userSettings == null) { UpdateUserSettings(); };
                return _userSettings;
            }
            set { _userSettings = value; }
        }

        public UserSettingsController()
        {
            Configuration.ConfigurationUpdated += UpdateUserSettings;
        }

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
            UserSettings = new UserSettings(username, email, directory);
            await SaveUserSettings(UserSettings);
        }
        internal async Task SetAndSaveUserSettings(UserSettings userSettings)
        {
            UserSettings = userSettings;
            await SaveUserSettings(userSettings);
        }
        internal UserSettings GetUserSettings()
        {
            return UserSettings.Clone();
        }
        internal bool IsInitialSettingsFilled()
        {
            return UserSettings.Directory.Length > 0;
        }
        private async void UpdateUserSettings()
        {
            _userSettings = await GetSpecificSetting();
        }
        private async Task SaveUserSettings(UserSettings userSettings)
        {
            if (userSettings == null) { throw new ArgumentNullException(); }
            await SetSpecificSetting(userSettings);
        }
    }
}
