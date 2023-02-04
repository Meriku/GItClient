using GItClient.Core;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using GItClient.MVVM.View.MainView;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GItClient.MVVM.ViewModel
{
    class UserInfoViewModel : IViewModel
    {
        public double MinWidth { get => 550; }
        public double MinHeight { get => 220; }

        public UserInfoViewModel()
        {
        }
    }
}
