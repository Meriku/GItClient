using GItClient.Core;
using GItClient.Core.Controllers;
using GItClient.MVVM.View;
using GItClient.MVVM.View.MainView;
using System.Windows;

namespace GItClient.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand HelloViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }    
        public RelayCommand ChangeMenuType { get; set; }
        public RelayCommand UserInfoViewCommand { get; set; }
        public RelayCommand InitRepoCommand { get; set; }

        public HelloViewModel HelloVM { get; set; }
        public HomeViewModel HomeVM { get; set; }
        public UserInfoViewModel UserInfoVM { get; set; }
        public InitRepoViewModel InitRepoVM { get; set; }

        private ViewHandler _currentView;
        private ViewHandler _currentMenu;
        private object _previousView;

        public object CurrentView
        {
            get { return _currentView.ViewModel; }
            set
            {
                _currentView.ViewModel = value;
                _currentView.RaiseOnViewChange();
                OnPropertyChanged();          
            }
        } 


        public object CurrentMenu
        {
            get { return _currentMenu.ViewModel; }
            set
            {
                _currentMenu.ViewModel = value;
                OnPropertyChanged();
            }

        }

        public MainViewModel() 
        {
            // TODO: remove redundant views and models

            _currentView = new ViewHandler();
            _currentMenu = new ViewHandler();

            CurrentView = new InitRepoViewModel(); //new HelloViewModel();
            CurrentMenu = new HomeMenuViewModel();
          
            _currentView.OnViewChange += ResizeWindow;
          
            HelloViewCommand = new RelayCommand(o =>
            {
                CurrentView = new HelloViewModel();
            });

            HomeViewCommand = new RelayCommand(o => 
            {
                CurrentView = new HomeViewModel();
            });

            InitRepoCommand = new RelayCommand(o =>
            {
                CurrentView = new InitRepoViewModel();
            });

            UserInfoViewCommand = new RelayCommand(o =>
            {
                if (CurrentView is UserInfoViewModel)
                {
                    CurrentView = _previousView ?? new HomeViewModel();
                }
                else
                {
                    _previousView = CurrentView;
                    CurrentView = new UserInfoViewModel();
                }
     
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

        private const int MarginHeight = 60;
        private const int MarginWidth = 15;
        private void ResizeWindow()
        {
            var window = Application.Current.MainWindow;
            window.MinHeight = ((IViewModel)CurrentView).MinHeight + MarginHeight;
            window.MinWidth = ((IViewModel)CurrentView).MinWidth + ((IViewModel)CurrentMenu).MinWidth + MarginWidth;
        }

    }
}
