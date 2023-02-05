using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;

namespace GItClient.Core.Controllers
{
    internal class GitController
    {
        // TODO: add logger for all git commands
        private ILogger _logger = LoggerProvider.GetLogger("GitController");

        private string? _gitVersion;

        private CircularList<CommandDateTime>? _commandsHistory;
        private CircularList<CommandDateTime> CommandsHistory
        {
            get
            {
                if (_commandsHistory == null) { _commandsHistory = new CircularList<CommandDateTime>(COMMANDS_HISTORY_LENGHT); };
                return _commandsHistory;
            }
            set { _commandsHistory = value; }

        }


        private const int COMMANDS_HISTORY_LENGHT = 10; // TODO: a const for now. Implement custom history lenght?

        internal string GetGitVersion()
        {
            if (_gitVersion != null) return _gitVersion;

            var results = ExecuteGitCommand(new string[] { "git version" });
            
            _gitVersion = ParseVersion(results[0]);
            return _gitVersion;  
        }

        internal bool InitRepository(string directory)
        {
            var results = ExecuteGitCommand(new string[] { $"cd {directory}", "git init" });

            return results.Count > 0;         
        }

        internal CommandDateTime[] GetCommandsHistory()
        {
            return CommandsHistory.GetReversed().ToArray();
        }

        private Collection<PSObject> ExecuteGitCommand(string[] commands)
        {           
            //TODO: current impl using PowerShell, change to exe git later?

            using PowerShell powershell = PowerShell.Create();
            foreach(var command in commands)
            {
                LogGitCommand(command);
                powershell.AddScript(command);
            }           
            var results = powershell.Invoke();
            //TODO: errors handling
            //TODO: convert to string[]
            return results;
        }

        private void LogGitCommand(string command)
        {
            AddCommandToHistory(command);
            _logger.LogDebug("Execute Git Command: " + command);
            WeakReferenceMessenger.Default.Send(new GitCommandChangedMessage(command));
        }


        private void LogGitCommandResult(string result)
        {
            //TODO: log in file
        }

        private void AddCommandToHistory(string command)
        {
            CommandsHistory.Add(new CommandDateTime(command));
        }

        private string ParseVersion(PSObject psObject)
        {
            return psObject.ToString()[12..];
        }

    }



}
