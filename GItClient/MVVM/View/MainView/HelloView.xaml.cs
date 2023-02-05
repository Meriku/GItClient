﻿using GItClient.Core.Controllers;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GItClient.MVVM.View.MainView
{
    /// <summary>
    /// Interaction logic for HelloView.xaml
    /// </summary>
    public partial class HelloView : UserControl
    {
        public HelloView()
        {
            InitializeComponent();


            UpdateGitVersion();
            UpdateClientVersion();
        }

        private void UpdateGitVersion()
        {
            Git_Version_Box.Text = "updating";
            var _gitController = ControllersProvider.GetGitController();
            Task.Run(() =>
            {
                var gitVersion = _gitController.GetGitVersion();
                this.Dispatcher.Invoke(() => {
                    Git_Version_Box.Text = gitVersion;
                });
            });
        }

        private void UpdateClientVersion()
        {
            var _appSettingsController = ControllersProvider.GetAppSettingsController();
            var clientVersion = _appSettingsController.GetAppVersion();

            Easy_Client_Version_Box.Text = clientVersion;
        }
    }
}
