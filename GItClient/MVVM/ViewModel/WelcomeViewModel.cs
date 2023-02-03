using GItClient.Core;
using GItClient.Core.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.MVVM.ViewModel
{
    public class WelcomeViewModel
    {
        private string _defaultDriveName;
        public string DefaultDriveName
        {
            get { return _defaultDriveName; }
            set { _defaultDriveName = value; }
        }

        public WelcomeViewModel()
        {
            var _userSettingsController = ControllersProvider.GetUserSettingsController();
            _defaultDriveName = _userSettingsController.GetDefaultDirectory();
        }

    }
}
