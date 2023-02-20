using GItClient.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.MVVM.ViewModel
{
    class CommitsHistoryPartialViewModel : IViewModel
    {
        public double MinWidth { get => 310; }
        public double MinHeight { get => 200; }

        public CommitsHistoryPartialViewModel(Repository repository)
        {
            // TODO : display info
        }

        public CommitsHistoryPartialViewModel()
        {
            // TODO : display info
        }
    }
}
