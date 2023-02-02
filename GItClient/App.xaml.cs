using GItClient.Core.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using System.Windows;

namespace GItClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            var userSettinsController = ControllersProvider.GetUserSettingsController();
            var profileCreated = userSettinsController.IsInitialSettingsFilled();

            this.StartupUri = profileCreated ?
                            new Uri("MainWindow.xaml", UriKind.Relative) :
                            new Uri("MVVM/Pages/Welcome.xaml", UriKind.Relative);

        }


    }


}
