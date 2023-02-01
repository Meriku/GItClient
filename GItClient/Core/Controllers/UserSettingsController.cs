using GItClient.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers
{
    public class UserSettingsController
    {
        private UserSettings _userSettings;

        public void SetUserSettings(string? username, string? email, string? directory)
        {
            _userSettings = new UserSettings(username, email, directory);
        }
        //TODO: save info into file
        public UserSettings GetUserSettings()
        {
            return _userSettings;
        }

    }
}
