using GItClient.Core.Controllers;
using GItClient.Core.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

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

            var _gitController = ControllersProvider.GetGitController();
            var gitVersion = await _gitController.GetGitVersionAsync();

            Git_Version_Box.Text = gitVersion;         
        }

        private void UpdateClientVersion()
        {
            var _appSettingsController = ControllersProvider.GetAppSettingsController();
            var clientVersion = _appSettingsController.GetAppVersion();

            Easy_Client_Version_Box.Text = clientVersion;
        }
    }
}
