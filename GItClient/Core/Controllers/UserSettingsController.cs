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

        //TODO: Write to file.
        public void SetUserSettings(string username, string email, string directory)
        {
            _userSettings = new UserSettings(username, email, directory);
        }

        public UserSettings GetUserSettings()
        {
            return _userSettings;
        }

    }
}
