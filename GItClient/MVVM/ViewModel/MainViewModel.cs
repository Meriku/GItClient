﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using GItClient.MVVM.View;
using GItClient.MVVM.View.MainView;
using System;
using System.Windows;

namespace GItClient.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    { 
        public RelayCommand UserInfoViewCommand { get; set; }

        public UserInfoViewModel UserInfoVM { get; set; }
        
        
        private object _currentView;
        private object _currentMenu;
        private object _previousView;

        private string _lastGitCommand;
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
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();          
            }
        } 

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

            WeakReferenceMessenger.Default.Register<GitCommandChangedMessage>(this, (r, m) =>
            { LastGitCommand = m.Value; });

            WeakReferenceMessenger.Default.Register<MainViewChangedMessage>(this, (r, m) =>
            { CurrentView = m.Value; });

            CurrentView = new HelloViewModel();
            CurrentMenu = new HomeMenuViewModel();               

            UserInfoViewCommand = new RelayCommand(() =>
            {
                if (CurrentView is UserInfoViewModel)
                {
                    WeakReferenceMessenger.Default.Send(new MainViewChangedMessage((IViewModel)_previousView));
                }
                else
                {
                    _previousView = CurrentView;
                    WeakReferenceMessenger.Default.Send(new MainViewChangedMessage(new UserInfoViewModel()));
                }  
            });               
        }
    }
}
