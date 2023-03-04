using GItClient.Core.Controllers;
using GItClient.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers.Static
{
    public static class Configuration
    {
        private static Settings? _configuration;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        private static readonly ILogger _logger = LoggerProvider.GetLogger("GitControllerBase");

        public static Action ConfigurationUpdated;
        /// <summary>
        /// Return cached _configuration if exist
        /// Or call method to read _configuration from the file
        /// </summary>

        internal static Task<Settings> GetConfiguration()
        {
            if (_configuration != null) return Task.FromResult(_configuration);

            var readTask = ReadConfiguration();

            return readTask;
        }

        internal static async Task SaveConfiguration(Settings configuration)
        {
            _configuration = configuration;
            await WriteConfiguration();

            ConfigurationUpdated?.Invoke();
        }

        /// <summary>
        /// Write new _configuration to the file
        /// And update cache
        /// </summary> 
        private static async Task WriteConfiguration()
        {
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
        private static async Task<Settings> ReadConfiguration()
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
