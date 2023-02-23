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

    public class UpdateGitHistoryMessage : ValueChangedMessage<int>
    {
        public UpdateGitHistoryMessage(int count) : base(count)
        {
        }
    }

    public class MainViewChangedMessage : ValueChangedMessage<IViewModel>
    {
        public MainViewChangedMessage(IViewModel viewmodel) : base(viewmodel)
        {
        }
    }

    public class RepositoryChangedMessage : ValueChangedMessage<Repository>
    {
        public RepositoryChangedMessage(Repository repository) : base(repository)
        {
        }
    }

    public class LoadCommitsMessage : ValueChangedMessage<Repository>
    {
        public LoadCommitsMessage(Repository repository) : base(repository)
        {
        }
    }
}
