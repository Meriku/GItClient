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

namespace GItClient.Core.Base
{
    internal class ConfigurationBase
    {
        private Settings? _configuration;
        private readonly SemaphoreSlim _semaphore;

        protected ConfigurationBase()
        {
            _semaphore = new SemaphoreSlim(1);
            _configuration = GetConfiguration();
        }

        protected Settings GetConfiguration()
        {
            if (_configuration != null) return _configuration;

            var readTask = ReadConfiguration();
            readTask.Wait();
            _configuration = readTask.Result;

            return _configuration;
        }
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
                //TODO add logger
            }
            finally
            {
                _semaphore.Release();
            }
        }
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
                //TODO add logger
            }
            finally
            {
                _semaphore.Release();
            }

            var settings = JsonConvert.DeserializeObject<Settings>(appsettings);

            if (settings == null || settings.AppSettings == null || settings.UserSettings == null)
                throw new Exception("Incorect appsettings file.");
                // TODO: restore default settings? 

            return settings;
        }
    }
}
