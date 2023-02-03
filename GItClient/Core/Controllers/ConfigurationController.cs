using GItClient.Core.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers
{
    internal class ConfigurationController
    {
        private Settings? _configuration;
        private SemaphoreSlim _semaphore;

        internal ConfigurationController()
        {
            _semaphore = new SemaphoreSlim(1);
            _configuration = GetConfiguration();
        }

        internal string GetDefaultDirectory()
        {
            var drives = DriveInfo.GetDrives();
            return drives[0].Name + "repos";
        }

        internal Settings GetConfiguration()
        {
            if (_configuration != null) return _configuration;

            var readTask = ReadConfiguration();
            readTask.Wait();
            _configuration = readTask.Result;

            return _configuration;
        }

        private async Task<Settings> ReadConfiguration()
        {
            var appsettings = String.Empty;

            await _semaphore.WaitAsync();
            try
            {
                 appsettings = File.ReadAllText("appsettings.json");
            }
            catch(Exception e)
            {
                //TODO add logger
            }
            finally
            {
                _semaphore.Release();
            }
            
            var settings = JsonConvert.DeserializeObject<Settings>(appsettings);

            if (settings == null) throw new Exception("Incorect appsettings file.");
            // TODO: restore default settings? 

            return settings;
        }

        internal async Task WriteConfiguration(Settings configuration)
        {
            var appsettings = JsonConvert.SerializeObject(configuration, Formatting.Indented);

            await _semaphore.WaitAsync();
            try
            {
                File.WriteAllText("appsettings.json", appsettings);
            }
            catch (Exception e)
            {
                //TODO add logger
            }
            finally
            {
                _semaphore.Release();
            }                     
        }
    }


    internal static class SettingsController<T> where T : ISetting
    {
        private static Settings? _configuration;
        private static ConfigurationController? _configurationController;

        public static T GetSpecificSetting()
        {
            _configurationController ??= ControllersProvider.GetConfigurationController(); 
            _configuration ??= _configurationController.GetConfiguration();

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

        public static async Task SetSpecificSetting(T setting)
        {
            _configurationController ??= ControllersProvider.GetConfigurationController();
            _configuration ??= _configurationController.GetConfiguration();

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

            await _configurationController.WriteConfiguration(_configuration);
        }

    }
}
