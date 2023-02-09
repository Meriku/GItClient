using CommunityToolkit.Mvvm.Messaging;
using GItClient.Core.Base;
using GItClient.Core.Models;
using Microsoft.Extensions.Logging;
using MS.WindowsAPICodePack.Internal;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace GItClient.Core.Controllers
{
    internal class GitController : GitControllerBase
    {
        private UserSettings UserSettings { get; set; }

        private UserSettingsController _userSettingsController; 

        protected string? _gitVersion;

        internal GitController()
        {
            _userSettingsController = ControllersProvider.GetUserSettingsController();
        }

        internal async Task<string> GetGitVersionAsync()
        {
            //if (_gitVersion != null) return _gitVersion;

            var request = new PowerShellCommands();
            request.AddCommand(CommandsPowerShell.git_Version);

            var results = await Execute(request);
            _gitVersion = ParseVersion(results.AllResponses.First() ?? "Error");
            // TODO: retry if Error? 

            return _gitVersion;  
        }

        internal async Task<bool> InitRepositoryAsync(string directory)
        {
            var request = new PowerShellCommands();
            request.AddCommand(CommandsPowerShell.git_Init, directory);

            var results = await ExecuteAndInformUIAsync(request);

            return results.IsError;         
        }

        internal async Task<bool> CloneRepositoryAsync(string directory, string link)
        {
            var request = new PowerShellCommands();
            request.AddCommand(CommandsPowerShell.git_Clone, new string[] {link, directory});

            var results = await Execute(request);

            return results.IsError;
        }

        internal async Task<bool> CreateFolderAsync(string directory, string folderName)
        {
            var request = new PowerShellCommands();
            request.AddCommand(CommandsPowerShell.cd, directory);
            request.AddCommand(CommandsPowerShell.md, folderName);

            var results = await Execute(request);
            return results.IsError;
        }

        internal string[] GetFormattedCommandsHistory(int count)
        {
            UserSettings = _userSettingsController.GetUserSettings();

            var result = new List<string>(count);

            foreach(var command in GitHistory.GetReversed())
            {
                if (command.Type == HistoryType.Response)
                {
                    if (UserSettings.Optional.ShowGitResponses)
                    {
                        result.Add(command.DateTime.ToString("T") + " " + command.Value + " \n");
                    }
                }
                else
                {
                    result.Add(command.DateTime.ToString("T") + " " + command.Value + " \n");
                }  

                if (result.Count >= count)
                {
                    return result.ToArray();
                }
            }

            return result.ToArray();
        }

        private string ParseVersion(string input)
        {
            return input[12..];
        }

    }



}
