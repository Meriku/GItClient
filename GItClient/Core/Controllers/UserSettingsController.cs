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

        internal async Task SetUserSettings(string? username, string? email, string? directory)
        {
            _userSettings = new UserSettings(username, email, directory);
            await SaveUserSettings();
        }

        internal UserSettings GetUserSettings()
        {
            return _userSettings ?? new UserSettings();
        }

        private async Task SaveUserSettings()
        {
            if (_userSettings == null) { throw new ArgumentNullException(); }

            var _configurationController = ControllersProvider.GetConfigurationController();

            await _configurationController.SetUserSettingsConfiguration(_userSettings);
        }

    }
}
