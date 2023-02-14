using GItClient.Core.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

namespace GItClient.Core.Controllers
{
    /// <summary>
    /// Contoller
    /// Provides Dialog to choose folder
    /// </summary>
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
