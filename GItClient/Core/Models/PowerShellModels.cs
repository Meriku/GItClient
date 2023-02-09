using GItClient.Core.Base;
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

        internal PowerShellCommand(CommandsPowerShell _command, string[] _arguments)
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

        public IEnumerable<string> AllCommands
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

        internal void AddCommand(CommandsPowerShell _command, string[] arguments = null)
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
        private const string ERROR_MESSAGE = "Error";
        internal string Response { get; private set; }
        internal bool IsError => Response.Contains(ERROR_MESSAGE);

        internal PowerShellResponse(string response)
        {
            Response = response;
        }
    }
    internal class PowerShellResponses
    {
        private PowerShellResponse[] Responses { get; set; }
        internal bool IsError => Responses.Any(x => x.IsError);
        internal string ErrorMessage => GetErrorMessage();
        public IEnumerable<string> AllResponses
        {
            get
            {
                foreach (var response in Responses)
                {
                    yield return response.Response;
                };
            }
        }
        internal PowerShellResponses() { }
        internal PowerShellResponses(PowerShellResponse[] responses)
        {
            Responses = responses;
        }

        private string GetErrorMessage()
        {
            if (IsError)
            {
                return Responses.First(x => x.IsError).ToString();
            }
            else
            {
                return String.Empty;
            }
        }
    }

}
