using GItClient.Core.Controllers;
using GItClient.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GItClient.Core.Base
{
    internal class ConfigurationBase
    {
        private Settings? _configuration;
        private readonly SemaphoreSlim _semaphore;
        private ILogger _logger = LoggerProvider.GetLogger("ConfigurationBase");

        protected ConfigurationBase()
        {
            _semaphore = new SemaphoreSlim(1);
            _configuration = GetConfiguration().Result;
        }

        /// <summary>
        /// Return cached _configuration if exist
        /// Or call method to read _configuration from the file
        /// </summary>
        protected Task<Settings> GetConfiguration()
        {
            if (_configuration != null) return Task.FromResult(_configuration);

            var readTask = ReadConfiguration();

            return readTask;
        }

        /// <summary>
        /// Write new _configuration to the file
        /// And update cache
        /// </summary>
        protected async Task WriteConfiguration(Settings configuration)
        {
            _configuration = configuration;
            var appsettings = JsonConvert.SerializeObject(_configuration, Formatting.Indented);

            await _semaphore.WaitAsync();
            try
            {
                File.WriteAllText("appsettings.json", appsettings);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Read _configuration from the file
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"> Incorrect appsettings.json </exception>
        private async Task<Settings> ReadConfiguration()
        {
            var appsettings = string.Empty;

            await _semaphore.WaitAsync();
            try
            {
                appsettings = File.ReadAllText("appsettings.json");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            finally
            {
                _semaphore.Release();
            }

            var settings = JsonConvert.DeserializeObject<Settings>(appsettings);

            if (settings == null || settings.AppSettings == null || settings.UserSettings == null)
            {
                _logger.LogError("Incorrect appsettings.json file.");
                throw new Exception("Incorrect appsettings file.");
            }               

            return settings;
        }
    }
}
