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

        internal Settings GetConfiguration()
        {
            if (_configuration != null) return _configuration;

            var readTask = ReadConfiguration();
            readTask.Wait();
            _configuration = readTask.Result;

            return _configuration;
        }

        internal async Task SetUserSettingsConfiguration(UserSettings settings)
        {
            if (_configuration == null) { throw new ArgumentNullException(); }

            _configuration.UserSettings = settings;

            await WriteConfiguration();
        }

        private async Task<Settings> ReadConfiguration()
        {
            var appsettings = String.Empty;

            await _semaphore.WaitAsync();
            try
            {
                 appsettings = await File.ReadAllTextAsync("appsettings.json");
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

        private async Task WriteConfiguration()
        {
            var appsettings = JsonConvert.SerializeObject(_configuration, Formatting.Indented);

            await _semaphore.WaitAsync();
            try
            {
                await File.WriteAllTextAsync("appsettings.json", appsettings);
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
}
