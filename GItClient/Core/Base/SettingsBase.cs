using GItClient.Core.Controllers;
using GItClient.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Base
{
    internal class SettingsBase<T> : ConfigurationBase where T : ISetting
    {
        private Settings? _configuration;

        protected T GetSpecificSetting()
        {
            _configuration ??= base.GetConfiguration();

            Type type = typeof(T);
            switch (type.Name)
            {
                case "UserSettings":
                    return (T)(ISetting)_configuration.UserSettings;
                case "AppSettings":
                    return (T)(ISetting)_configuration.AppSettings;
                default:
                    throw new NotImplementedException();
            }
        }
        protected async Task SetSpecificSetting(T setting)
        {
            _configuration ??= base.GetConfiguration();

            Type type = typeof(T);
            switch (type.Name)
            {
                case "UserSettings":
                    _configuration.UserSettings = (UserSettings)(ISetting)setting;
                    break;
                case "AppSettings":
                    _configuration.AppSettings = (AppSettings)(ISetting)setting;
                    break;
                default:
                    throw new NotImplementedException();
            }

            await base.WriteConfiguration(_configuration);
        }
    }
}
