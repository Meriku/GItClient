using GItClient.Core.Controllers;
using GItClient.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers.Static
{
    internal static class PowerShellExecutor
    {
        private static readonly ILogger _logger = LoggerProvider.GetLogger("PowerShellBase");
        
        private static PowerShellResponse? ErrorResponse;

        internal static readonly SemaphoreSlim IsWorking = new SemaphoreSlim(1);

        internal static event EventHandler<PowerShellResponses> DataAdded;


        /// <summary>
        /// Create PowerShell instance
        /// and execute the command
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static async Task<PowerShellResponses> Execute(PowerShellCommands commands)
        {
            ErrorResponse = null;
            try
            {
                return await Task.Run(async () =>
                {
                    using PowerShell powershell = PowerShell.Create();

                    powershell.Runspace.SessionStateProxy.PSVariable.Set("OutputEncoding", System.Text.Encoding.UTF8);

                    foreach (var command in commands.AllCommands)
                    {
                        powershell.AddScript(command);
                    }

                    if (!commands.InternalUsage)
                    {
                        powershell.Streams.Debug.DataAdded += PowerShell_DataAdded;
                        powershell.Streams.Error.DataAdded += PowerShell_DataAdded;
                        powershell.Streams.Information.DataAdded += PowerShell_DataAdded;
                        powershell.Streams.Progress.DataAdded += PowerShell_DataAdded;
                        powershell.Streams.Verbose.DataAdded += PowerShell_DataAdded;
                        powershell.Streams.Warning.DataAdded += PowerShell_DataAdded;
                    }
  
                    var responses = await powershell.InvokeAsync();
                    var result = await ParseResponse(responses, commands);

                    return result;
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            throw new Exception("Failed execution of PowerShell Command.");
        }

        /// <summary>
        /// Converts PSDataCollection to PowerShellResponse
        /// </summary>
        /// <param name="PSObjects"></param>
        /// <returns></returns>
        private static async Task<PowerShellResponses> ParseResponse(PSDataCollection<PSObject> PSObjects, PowerShellCommands commands)
        {
            if (ErrorResponse != null) { return new PowerShellResponses(ErrorResponse); }

            var collection = PSObjects.ReadAll();

            if (collection.Count == 0) { return new PowerShellResponses(); }

            var result = new List<PowerShellResponse>();
            for (var i = 0; i < collection.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(collection[i].ToString()))
                {
                    result.Add(new PowerShellResponse(collection[i].ToString().Trim(), ResponseType.Successful));
                }
                
            }

            if (!commands.InternalUsage)
            {
                InformUI(result);
            }


            return new PowerShellResponses(result);
        }

        /// <summary>
        /// Invokes when new message is added to powershell.Streams
        /// Converts PSDataCollection to PowerShellResponse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception">Unknown type of PSDataCollection</exception>
        private static void PowerShell_DataAdded(object? sender, DataAddedEventArgs e)
        {
            var result = new List<PowerShellResponse>();
            switch (sender)
            {
                case PSDataCollection<DebugRecord> debugRecords:
                        result.Add(new PowerShellResponse(debugRecords[e.Index].Message, ResponseType.Debug));
                    break;

                case PSDataCollection<ErrorRecord> errorRecords:
                        ErrorResponse = new PowerShellResponse(errorRecords[e.Index].Exception.Message, ResponseType.Error);
                        result.Add(ErrorResponse);
                    break;

                case PSDataCollection<InformationRecord> informationRecords:
                        result.Add(new PowerShellResponse(informationRecords[e.Index].MessageData.ToString(), ResponseType.Information));
                    break;

                case PSDataCollection<ProgressRecord> progressRecords:
                        result.Add(new PowerShellResponse(progressRecords[e.Index].PercentComplete.ToString(), ResponseType.Progress));
                    break;

                case PSDataCollection<VerboseRecord> verboseRecords:
                        result.Add(new PowerShellResponse(verboseRecords[e.Index].Message, ResponseType.Verbose));                   
                    break;

                case PSDataCollection<WarningRecord> warningRecords:
                        result.Add(new PowerShellResponse(warningRecords[e.Index].Message, ResponseType.Warning));

                    break;
                default:
                    throw new Exception("Unknown type of PSDataCollection");
            }

            InformUI(result);
        }

        /// <summary>
        /// If messages count > 0 invokes event
        /// inheritor of the base class has an ability
        /// to subscribe on this event and use PowerShell responses
        /// </summary>
        /// <param name="result"></param>
        private static void InformUI(List<PowerShellResponse> result)
        {
            if (result.Count > 0)
            {
                DataAdded?.Invoke(null, new PowerShellResponses(result));
            }
        }
    }

    enum CommandsPowerShell
    {
        cd,
        md,
        git_Version,
        git_Init,
        git_Clone,
        git_Log,
        git_Revparse
    }
}
