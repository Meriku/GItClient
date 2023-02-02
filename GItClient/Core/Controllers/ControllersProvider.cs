using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers
{
    internal static class ControllersProvider
    {

        private static UserSettingsController? _userSettingsController;
        private static ConfigurationController? _configurationController;

        public static UserSettingsController GetUserSettingsController()
        {
            _userSettingsController ??= new UserSettingsController();
            return _userSettingsController;
        }

        public static ConfigurationController GetConfigurationController()
        {
            _configurationController ??= new ConfigurationController();
            return _configurationController;
        }

    }
}
