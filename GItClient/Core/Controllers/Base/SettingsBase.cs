using GItClient.Core.Controllers.Static;
using GItClient.Core.Models;
using System;
using System.Threading.Tasks;

namespace GItClient.Core.Base
{

    /// <summary>
    /// Get or Set Specific Setting from _configuration
    /// Generic class which gives particular Setting
    /// All calls to settings should be made using this class
    /// </summary>
    internal class SettingsBase<T> where T : ISetting
    {
        private Settings? _configuration;

        protected async Task<T> GetSpecificSetting()
        {
            _configuration ??= await Configuration.GetConfiguration();
            T result = default;

            Type type = typeof(T);
            switch (type.Name)
            {
                case "UserSettings":
                    result = (T)(ISetting)_configuration.UserSettings;
                    break;
                case "AppSettings":
                    result = (T)(ISetting)_configuration.AppSettings;
                    break;
                case "RepositorySettings":
                    result = (T)(ISetting)_configuration.RepositorySettings;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return result;
        }
        protected async Task SetSpecificSetting(T setting)
        {
            _configuration ??= await Configuration.GetConfiguration();

            Type type = typeof(T);
            switch (type.Name)
            {
                case "UserSettings":
                    _configuration.UserSettings = (UserSettings)(ISetting)setting;
                    break;
                case "AppSettings":
                    _configuration.AppSettings = (AppSettings)(ISetting)setting;
                    break;
                case "RepositorySettings":
                    _configuration.RepositorySettings = (RepositorySettings)(ISetting)setting;
                    break;
                default:
                    throw new NotImplementedException();
            }

            await Configuration.SaveConfiguration(_configuration);
        }
    }
}
