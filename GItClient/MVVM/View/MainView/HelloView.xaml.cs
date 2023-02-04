using GItClient.Core.Controllers;
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

            var gitController = ControllersProvider.GetGitController();
            var appSettingsController = ControllersProvider.GetAppSettingsController();

            var gitVersion = gitController.GetGitVersion();
            var clietnVersion = appSettingsController.GetAppVersion();

            Git_Version_Box.Text = gitVersion;
            Easy_Client_Version_Box.Text = clietnVersion;



        }
    }
}
