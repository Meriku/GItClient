using GItClient.Core.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers
{
    class DirectoryController
    {
        private UserSettings UserSettings;
        private UserSettingsController _userSettingsController;

        public DirectoryController()
        {
            _userSettingsController = ControllersProvider.GetUserSettingsController();            
        }

        internal CommonOpenFileDialog GetDirectoryDialog(string path = "")
        {
            UserSettings = _userSettingsController.GetUserSettings();
            if (string.IsNullOrEmpty(path))
            {
                path = UserSettings.Directory;
            }
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = Directory.Exists(path) ? path : _userSettingsController.GetDefaultDrive()
            };
            return dialog;
        }
    }
}
