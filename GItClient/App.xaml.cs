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

            var profileCreated = false;

            //TODO: If profile created - skip.
            if (profileCreated)
            {

            }
            else
            {
                this.StartupUri = new Uri("MVVM/Pages/Welcome.xaml", UriKind.Relative);
            }

            
        }


    }


}
