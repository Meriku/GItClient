using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using MS.WindowsAPICodePack.Internal;
using System.Threading;
using CommunityToolkit.Mvvm.Messaging;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using GItClient.Core.Models;
using System.IO;
using Microsoft.Extensions.Logging;

namespace GItClient.Core.Controllers
{
    internal class GitController
    {
        // TODO: add logger for all git commands
        private ILogger _logger = LoggerProvider.GetLogger("GitController");
        private string? _gitVersion;

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
            _logger.LogDebug("Execute Git Command: " + command);
            WeakReferenceMessenger.Default.Send(new GitCommandChangedMessage(command));
        }

        private void LogGitCommandResult(string result)
        {
            //TODO: log in file
        }

        private string ParseVersion(PSObject psObject)
        {
            return psObject.ToString()[12..];
        }

    }
}
