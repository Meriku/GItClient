using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using GItClient.MVVM.View;
using GItClient.MVVM.View.MainView;
using System.Windows;

namespace GItClient.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    { 
        public RelayCommand UserInfoViewCommand { get; set; }

        public UserInfoViewModel UserInfoVM { get; set; }

        private ViewHandler _currentView;
        private ViewHandler _currentMenu;
        private string _lastGitCommand;
        private object _previousView;

        public string LastGitCommand
        {
            get { return _lastGitCommand; }
            set
            {
                _lastGitCommand = value;
                OnPropertyChanged();
            }
        }

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

            WeakReferenceMessenger.Default.Register<GitCommandChangedMessage>(this, (r, m) =>
            { LastGitCommand = m.Value; });

            WeakReferenceMessenger.Default.Register<MainViewChangedMessage>(this, (r, m) =>
            { CurrentView = m.Value; });


            _currentView = new ViewHandler();
            _currentMenu = new ViewHandler();

            CurrentView = new HelloViewModel();
            CurrentMenu = new HomeMenuViewModel();
          
            _currentView.OnViewChange += ResizeWindow;
          

            UserInfoViewCommand = new RelayCommand(() =>
            {
                if (CurrentView is UserInfoViewModel)
                {
                    CurrentView = _previousView ?? new HelloViewModel();
                }
                else
                {
                    _previousView = CurrentView;
                    CurrentView = new UserInfoViewModel();
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
