using CommunityToolkit.Mvvm.Messaging.Messages;
using GItClient.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace GItClient.Core.Models
{
    internal class Messages
    {
    }

    public class GitCommandChangedMessage : ValueChangedMessage<string>
    {
        public GitCommandChangedMessage(string command) : base(command)
        {
        }
    }

    public class GitCommandsHistoryMessage : ValueChangedMessage<int>
    {
        public GitCommandsHistoryMessage(int count) : base(count)
        {
        }
    }

    public class MainViewChangedMessage : ValueChangedMessage<IViewModel>
    {
        public MainViewChangedMessage(IViewModel viewmodel) : base(viewmodel)
        {
        }
    }
}
