using GItClient.Core.Base;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Models
{
    internal class PowerShellCommand
    {
        /// <summary>
        /// Create PowerShellCommand, converts Enum CommandsPowerShell 
        /// into ready for PowerShell string, adds arguments if needed
        /// </summary>
        private CommandsPowerShell command { get; set; }
        private string[] arguments { get; set; }

        public string Command { get; private set; }

        internal PowerShellCommand(CommandsPowerShell _command, string[]? _arguments)
        {
            command = _command;
            arguments = _arguments ?? System.Array.Empty<string>();

            Command = command.ToPWString();
            foreach (var arg in arguments)
            {
                Command += " " + arg.AddBracketsIfSpaces();
            }
        }

        internal PowerShellCommand(CommandsPowerShell _command, string _argument)
        {
            command = _command;
            arguments = _argument != null ? new string[1] { _argument } : System.Array.Empty<string>();

            Command = command.ToPWString();
            foreach (var arg in arguments)
            {
                Command += " " + arg.AddBracketsIfSpaces();
            }
        }
    }
    internal class PowerShellCommands
    {
        private PowerShellCommand[] AllCommandsInternal { get; set; }

        internal IEnumerable<string> AllCommands
        {
            get
            {
                foreach (var internalCommand in AllCommandsInternal)
                {
                    yield return internalCommand.Command;
                };
            }
        }

        private int LastCommandIndex;

        internal PowerShellCommands(int count = 1)
        {
            AllCommandsInternal = new PowerShellCommand[count];
            LastCommandIndex = 0;
        }

        internal void AddCommand(CommandsPowerShell _command, string?[] arguments = null)
        {
            if (LastCommandIndex > AllCommandsInternal.Length)
            {
                throw new System.Exception("Index was outside the bounds of the array.");
            }
            else
            {
                AllCommandsInternal[LastCommandIndex] = new PowerShellCommand(_command, arguments);
                LastCommandIndex++;
            }
        }
        internal void AddCommand(CommandsPowerShell _command, string argument)
        {
            if (LastCommandIndex > AllCommandsInternal.Length)
            {
                throw new System.Exception("Index was outside the bounds of the array.");
            }
            else
            {
                AllCommandsInternal[LastCommandIndex] = new PowerShellCommand(_command, argument);
                LastCommandIndex++;
            }
        }
    }
    internal class PowerShellResponse
    {
        internal string Message { get; private set; }
        internal ResponseType Type { get; private set; }

        internal PowerShellResponse(string response, ResponseType type)
        {
            Message = response;
            Type = type;
        }
    }
    internal class PowerShellResponses
    {
        private List<PowerShellResponse> Responses { get; set; }
        internal bool IsError => Responses.Any(x => x.Type == ResponseType.Error);

        internal IEnumerable<PowerShellResponse> AllResponses
        {
            get
            {
                foreach (var response in Responses)
                {
                    yield return response;
                };
            }
        }
        internal PowerShellResponses() { Responses = new List<PowerShellResponse>(); }

        internal PowerShellResponses(PowerShellResponse response) 
        { 
            Responses = new List<PowerShellResponse>() { response };
            
        }
        internal PowerShellResponses(List<PowerShellResponse> responses)
        {
            Responses = responses;
        }

        internal void AddResponse(PowerShellResponse newResponse)
        {
            Responses.Add(newResponse);
        }

    }
    enum ResponseType
    {
        Debug,
        Error,
        Information,
        Progress,
        Verbose,
        Warning,
        Successful
    }


}
