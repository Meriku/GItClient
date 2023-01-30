using GItClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;

namespace GItClient.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand HelloViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand DiscoveryViewCommand { get; set; }
        public RelayCommand ChangeMenuType { get; set; }


        public HelloViewModel HelloVM { get; set; }
        public HomeViewModel HomeVM { get; set; }
        public DiscoveryViewModel DiscoveryVM { get; set; }


        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private object _currentMenu;

        public object CurrentMenu
        {
            get { return _currentMenu; }
            set
            {
                _currentMenu = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel() 
        {
            CurrentView = new HelloViewModel();
            CurrentMenu = new ActionMenuViewModel();

            HelloViewCommand = new RelayCommand(o =>
            {
                CurrentView = new HelloViewModel();
            });

            HomeViewCommand = new RelayCommand(o => 
            {
                CurrentView = new HomeViewModel();
            });

            DiscoveryViewCommand = new RelayCommand(o =>
            {
                CurrentView = new DiscoveryViewModel();
            });

            ChangeMenuType = new RelayCommand(o =>
            {
                if (CurrentMenu is ActionMenuViewModel)
                {
                    CurrentMenu = new NavigateMenuViewModel();
                }
                else
                {
                    CurrentMenu = new ActionMenuViewModel();
                }
            });

        }

    }
}
