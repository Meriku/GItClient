using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Models
{


    public class Settings
    {

    }

    public class UserSettings
    {
        public UserSettings(string? username, string? email, string? directory) 
        {
            _username = username;
            _email = email;
            _directory = directory;
        }

        private string? _username;
        private string? _email;
        private string? _directory;

        public override string ToString()
        {
            return _username + " " + _email + " " + _directory;
        }
    }
}
