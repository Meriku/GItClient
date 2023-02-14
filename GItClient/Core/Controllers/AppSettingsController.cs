using GItClient.Core.Base;
using GItClient.Core.Models;

namespace GItClient.Core.Controllers
{
    /// <summary>
    /// Controller
    /// Easy way to access AppSettings
    /// Model: public class AppSettings : ISetting
    /// </summary>
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

        private async void UpdateAppSettings()
        {
            _appSettings = await base.GetSpecificSetting();
        }

    }
}
