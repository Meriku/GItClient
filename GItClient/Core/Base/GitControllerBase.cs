using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Controllers;
using GItClient.Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GItClient.Core.Base
{
    internal class GitControllerBase : PowerShellBase
    {
        private const int COMMANDS_HISTORY_LENGHT = 25;

        private CircularList<HistoryElement>? _gitHistory;
        private CircularList<HistoryElement> GitHistory
        {
            get
            {
                if (_gitHistory == null) { _gitHistory = new CircularList<HistoryElement>(COMMANDS_HISTORY_LENGHT); };
                return _gitHistory;
            }
            set { _gitHistory = value; }

        }
        
        private ILogger _logger = LoggerProvider.GetLogger("GitControllerBase");

        private UserSettings UserSettings { get; set; }

        private UserSettingsController _userSettingsController;

        protected GitControllerBase()
        {
            _userSettingsController = ControllersProvider.GetUserSettingsController();
            base.DataAdded += AddResponseToHistory;
        }

        /// <summary>
        /// Call method to add command to history and update UI
        /// Call base method to execute command
        /// Return result
        /// </summary>
        /// <param name="commands">Commands to execute</param>
        /// <returns></returns>
        protected async Task<PowerShellResponses> ExecuteAndInformUIAsync(PowerShellCommands commands)
        {
            AddRequestToHistory(commands);

            var result = await Execute(commands);

            return result;
        }


        /// <summary>
        /// Adds response to the history (circular list)
        /// and sends message to update UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="responses"></param>
        private void AddResponseToHistory(object? sender, PowerShellResponses responses)
        {
            if (sender == null) return;

            foreach (var response in responses.AllResponses)
            {
                GitHistory.Add(new HistoryElement(response.Message, HistoryType.Response));
                _logger.LogDebug("Received Git Response: " + response.Message);
            }
            StrongReferenceMessenger.Default.Send(new UpdateGitHistoryMessage(0));
        }

        /// <summary>
        /// Adds request to the history (circular list)
        /// and sends message to update UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="responses"></param>
        private void AddRequestToHistory(PowerShellCommands requests)
        {
            foreach (var request in requests.AllCommands)
            {
                GitHistory.Add(new HistoryElement(request, HistoryType.Request));
                _logger.LogDebug("Executed Git Command: " + request);
            }
            StrongReferenceMessenger.Default.Send(new UpdateGitHistoryMessage(0));
        }

        /// <summary>
        /// Using UserSettings, returns git history with responses or not
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        internal Task<string[]> GetFormattedCommandsHistory(int count = 1)
        {
            UserSettings = _userSettingsController.GetUserSettings();

            var result = new List<string>(count);

            foreach (var command in GitHistory.GetReversed())
            {
                switch(command.Type)
                {
                    case HistoryType.Request:
                        result.Add(command.DateTime.ToString("T") + " " + command.Value);
                        break;
                    case HistoryType.Response:
                        if (UserSettings.Optional.ShowGitResponses) { result.Add(command.DateTime.ToString("T") + " " + command.Value); }
                        break;
                }

                if (result.Count >= count)
                {
                    return Task.FromResult(result.ToArray());
                }
            }

            return Task.FromResult(result.ToArray());
        }
    }
}
