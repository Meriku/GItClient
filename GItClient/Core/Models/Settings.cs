using GItClient.Core.Controllers;
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
            AppName = appName;
            AppVersion = appVersion;
        }

        public string AppName { get; private set; }
        public string AppVersion { get; private set; }
    }

    public class UserSettings : ISetting
    {
        public UserSettings(string? username = null, string? email = null, string? directory = "") 
        {
            if (directory == null || directory.Equals("")) throw new ArgumentNullException("Directory can not be empty");

            Username = username ?? "";
            Email = email ?? "";
            Directory = directory;
        }

        public string Username { get; private set; }
        public string Email { get; private set; }
        public string Directory { get; private set; }

        public override string ToString()
        {
            return Username + " " + Email + " " + Directory;
        }
    }
}
