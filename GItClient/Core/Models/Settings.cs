using GItClient.Core.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Models
{

    internal interface ISetting
    {
    }

    public class Settings
    {
        public AppSettings? AppSettings { get; set; }
        public UserSettings? UserSettings { get; set; }
        public RepositorySettings RepositorySettings { get; set; }

        public Settings() { }
    }

    public class AppSettings : ISetting
    {
        public AppSettings(string appName, string appVersion) 
        {
            AppName = appName;
            AppVersion = appVersion;
        }

        public string AppName { get; private set; }
        public string AppVersion { get; private set; }
    }

    public class UserSettings : ISetting
    {
        public UserSettings(string? username = null, string? email = null, string? directory = null) 
        {
            Username = username ?? "";
            Email = email ?? "";
            Directory = directory ?? "";
            Optional = new UserSettingsOptional();
        }

        public string Username { get; set; }
        public string Email { get; set; }
        public string Directory { get; set; }
        public UserSettingsOptional Optional { get; set; }

        public UserSettings Clone()
        {
            var cloned = new UserSettings(this.Username, this.Email, this.Directory);
            cloned.Optional = this.Optional;
            return cloned;
        }

        public override string ToString()
        {
            return Username + " " + Email + " " + Directory;
        }
    }
    public class UserSettingsOptional
    {
        public bool ShowGitResponses { get; set; }
        public bool two { get; set; }
        public bool three { get; set; }
    }

    public class RepositorySettings : ISetting
    {
        public RepositoryImage[] ActiveRepositories { get; set; }

        public RepositorySettings() { }
        public RepositorySettings(RepositoryImage[] repos) 
        { 
            ActiveRepositories = repos;
        }

    }


}
