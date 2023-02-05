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

        private AppSettings AppSettings 
        { 
            get
            {
                if (_appSettings == null) { UpdateAppSettings(); };
                return _appSettings;
            }
            set { _appSettings = value; } 
        }

        internal AppSettings GetAppSettings()
        {
            return AppSettings;
        }

        internal string GetAppVersion()
        {
            return AppSettings.AppVersion;
        }

        private void UpdateAppSettings()
        {
            _appSettings = base.GetSpecificSetting();
        }

    }
}
