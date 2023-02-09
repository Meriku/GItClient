using GItClient.Core.Controllers;
using GItClient.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.PowerShell.Commands.Utility;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GItClient.Core.Base
{
    internal class PowerShellBase
    {
        private ILogger _logger = LoggerProvider.GetLogger("PowerShellBase");

        protected async Task<PowerShellResponses> Execute(PowerShellCommands commands)
        {
            var result = await ExecuteImplementation(commands);

            if (result == null) 
            { _logger.LogInformation("PowerShell Command returned null while executing PowerShellCommands"); }

            else if (result.IsError)
            { _logger.LogInformation($"PowerShell Command returned Error while executing PowerShellCommands: {result.ErrorMessage}"); }

            return result;
        }

        private async Task<PowerShellResponses> ExecuteImplementation(PowerShellCommands commands)
        {
            PowerShellResponses? result = null;
            try
            {
                await Task.Run(() =>
                { 
                    using PowerShell powershell = PowerShell.Create();
                    foreach (var command in commands.AllCommands)
                    {
                        //TODO: LogGitCommand(command);
                        powershell.AddScript(command);
                    }
                    var responses = powershell.Invoke();
                    result = ParseResult(responses);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return result;
        }

        private PowerShellResponses ParseResult(Collection<PSObject> input)
        {
            var result = new PowerShellResponse[input.Count];
            for (var i = 0; i < input.Count; i++)
            {
                result[i] = new PowerShellResponse(input[i].ToString());
            }
            return new PowerShellResponses(result);
        }
    }

    enum CommandsPowerShell
    {
        cd,
        md,
        git_Version,
        git_Init,
        git_Clone
    }
}
