using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers
{
    internal static class ControllersProvider
    {

        private static UserSettingsController? _userSettingsController;

        public static UserSettingsController GetUserSettingsController()
        {
            return _userSettingsController ?? new UserSettingsController();
        }

    }
}
