using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Models;
using GItClient.MVVM.View.MainView;
using GItClient.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GItClient.MVVM.View.Menus
{
    /// <summary>
    /// Interaction logic for HomeMenuView.xaml
    /// </summary>
    public partial class HomeMenuView : UserControl
    {
        public HomeMenuView()
        {
            InitializeComponent();
        }

        private void button_InitRepo(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new MainViewChangedMessage(new InitRepoViewModel()));
        }
        private void button_CloneRepo(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new MainViewChangedMessage(new CloneRepoViewModel()));
        }

        private void button_Home(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new MainViewChangedMessage(new HelloViewModel()));
        }     

        private void button_Exit(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }




}
