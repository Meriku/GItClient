using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Base
{
    internal class GitControllerBase : PowerShellBase
    {
        private const int COMMANDS_HISTORY_LENGHT = 25;
        private CircularList<HistoryElement>? _gitHistory;
        protected CircularList<HistoryElement> GitHistory
        {
            get
            {
                if (_gitHistory == null) { _gitHistory = new CircularList<HistoryElement>(COMMANDS_HISTORY_LENGHT); };
                return _gitHistory;
            }
            set { _gitHistory = value; }

        }
        
        private ILogger _logger = LoggerProvider.GetLogger("GitControllerBase");

        protected async Task<PowerShellResponses> ExecuteAndInformUIAsync(PowerShellCommands commands)
        {
            var result = await Execute(commands);

            LogGitCommands(commands);
            LogGitCommandsResult(result);

            WeakReferenceMessenger.Default.Send(new UpdateGitHistoryMessage(1));

            return result;
        }

        private void LogGitCommands(PowerShellCommands requests)
        {
            foreach (var request in requests.AllCommands)
            {
                GitHistory.Add(new HistoryElement(request, HistoryType.Request));
                _logger.LogDebug("Executed Git Command: " + request);
            }                  
        }

        private void LogGitCommandsResult(PowerShellResponses result)
        {
            foreach (var response in result.AllResponses)
            {
                GitHistory.Add(new HistoryElement(response, HistoryType.Response));
                _logger.LogDebug("Git Response: " + response);
            }
        }

    }
}
