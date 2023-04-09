using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using Microsoft.WindowsAPICodePack.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using Task = System.Threading.Tasks.Task;

namespace GItClient.MVVM.View.PartialView
{
    /// <summary>
    /// Interaction logic for CommitsHistoryTreeView.xaml
    /// </summary>
    public partial class CommitsHistoryTreeView : UserControl
    {
        public CommitsHistoryTreeView()
        {
            InitializeComponent();
        }
    }
}

