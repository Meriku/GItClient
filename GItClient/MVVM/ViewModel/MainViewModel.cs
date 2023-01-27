using GItClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand DiscoveryViewCommand { get; set; }

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

        public MainViewModel() 
        {
            CurrentView = new HomeViewModel();

            HomeViewCommand = new RelayCommand(o => 
            {
                CurrentView = new HomeViewModel();
            });

            DiscoveryViewCommand = new RelayCommand(o =>
            {
                CurrentView = new DiscoveryViewModel();
            });
        }

    }
}
