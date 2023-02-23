using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.MVVM.ViewModel
{
    internal class CommitsHistoryViewModel : ObservableObject, IViewModel
    {
        public double MinWidth { get => 330; }
        public double MinHeight { get => 250; }

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

        public CommitsHistoryViewModel()
        {

            //var repo = new Repository();
            //repo.Path = "C:\\Users\\Myroslav\\source\\repos\\WebPage_Online-Pizzeria";

            //CurrentView = new CommitsHistoryPartialViewModel();

            CurrentView = new CommitsHistoryPartialViewModel();

            WeakReferenceMessenger.Default.Register<RepositoryChangedMessage>(this, (r, m) =>
            { CurrentView = new CommitsHistoryPartialViewModel(); });
        }

    }
}
