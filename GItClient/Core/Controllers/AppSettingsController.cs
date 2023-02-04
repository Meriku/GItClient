using GItClient.Core.Base;
using GItClient.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers
{
    internal class AppSettingsController : SettingsBase<AppSettings>
    {
        private AppSettings? _appSettings;

        internal AppSettings GetAppSettings()
        {
            UpdateAppSettings();
            return _appSettings;
        }

        internal string GetAppVersion()
        {
            UpdateAppSettings();
            return _appSettings.AppVersion;
        }

        private void UpdateAppSettings()
        {
            _appSettings ??= base.GetSpecificSetting();
        }

    }
}
