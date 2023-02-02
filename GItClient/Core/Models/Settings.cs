using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Models
{

    public interface ISetting
    {
    }

    public class Settings
    {
        public AppSettings AppSettings { get; set; }
        public UserSettings UserSettings { get; set; }

        public Settings() { }
    }

    public class AppSettings : ISetting
    {
        public AppSettings(string appName, string appVersion) 
        {
            _appName = appName;
            _appVersion = appVersion;
        }

        public string _appName { get; private set; }
        public string _appVersion { get; private set; }
    }

    public class UserSettings : ISetting
    {
        public UserSettings(string? username = null, string? email = null, string? directory = null) 
        {
            Username = username;
            Email = email;
            Directory = directory;
        }

        private string _username;
        private string _email;
        private string _directory;

        public string? Username { get => _username ?? ""; private set => _username = value ?? ""; }
        public string? Email { get => _email ?? ""; private set => _email = value ?? ""; }
        public string? Directory { get => _directory ?? ""; private set => _directory = value ?? ""; }


        public override string ToString()
        {
            return Username + " " + Email + " " + Directory;
        }
    }
}
