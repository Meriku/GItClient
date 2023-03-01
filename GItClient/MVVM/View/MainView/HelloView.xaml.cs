using GItClient.Core.Controllers;
using GItClient.Core.Controllers.SettingControllers;
using System.ComponentModel;
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

            if (DesignerProperties.GetIsInDesignMode(this)) return;
            
            UpdateGitVersion();
            UpdateClientVersion();
        }

        private async void UpdateGitVersion()
        {
            Git_Version_Box.Text = "updating";

            var _gitController = new GitController();
            var gitVersion = await _gitController.GetGitVersionAsync();

            Git_Version_Box.Text = gitVersion;         
        }

        private void UpdateClientVersion()
        {
            var _appSettingsController = new AppSettingsController();
            var clientVersion = _appSettingsController.GetAppVersion();

            Easy_Client_Version_Box.Text = clientVersion;
        }
    }
}
