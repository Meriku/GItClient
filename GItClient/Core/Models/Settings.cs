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
        public AppSettings AppSettings { get; set; }
        public UserSettings UserSettings { get; set; }

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
        }

        public string Username { get; set; }
        public string Email { get; set; }
        public string Directory { get; set; }

        public UserSettings Clone()
        {
            return new UserSettings(this.Username, this.Email, this.Directory);
        }

        public override string ToString()
        {
            return Username + " " + Email + " " + Directory;
        }
    }
}
