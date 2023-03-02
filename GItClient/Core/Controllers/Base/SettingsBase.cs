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
        protected async Task<T> GetSpecificSetting()
        {
            var configuration = await Configuration.GetConfiguration();
            T result = default;

            Type type = typeof(T);
            switch (type.Name)
            {
                case "UserSettings":
                    result = (T)(ISetting)configuration.UserSettings;
                    break;
                case "AppSettings":
                    result = (T)(ISetting)configuration.AppSettings;
                    break;
                case "RepositorySettings":
                    result = (T)(ISetting)configuration.RepositorySettings;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return result;
        }
        protected async Task SetSpecificSetting(T setting)
        {
            var configuration = await Configuration.GetConfiguration();

            Type type = typeof(T);
            switch (type.Name)
            {
                case "UserSettings":
                    configuration.UserSettings = (UserSettings)(ISetting)setting;
                    break;
                case "AppSettings":
                    configuration.AppSettings = (AppSettings)(ISetting)setting;
                    break;
                case "RepositorySettings":
                    configuration.RepositorySettings = (RepositorySettings)(ISetting)setting;
                    break;
                default:
                    throw new NotImplementedException();
            }

            await Configuration.SaveConfiguration(configuration);
        }
    }
}
