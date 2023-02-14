using GItClient.Core.Base;
using GItClient.Core.Models;
using System.Linq;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers
{
    /// <summary>
    /// Controller 
    /// Provides "API" for Git commands
    /// </summary>
    internal class GitController : GitControllerBase
    {
        protected string? _gitVersion;

        internal async Task<string> GetGitVersionAsync()
        {
            if (_gitVersion != null) return _gitVersion;

            var request = new PowerShellCommands();
            request.AddCommand(CommandsPowerShell.git_Version);

            var results = await ExecuteAndInformUIAsync(request);

            _gitVersion = ParseVersion(results); 

            return _gitVersion;  
        }

        private string ParseVersion(PowerShellResponses input)
        {
            if (!input.AllResponses.Any()) { return "Error"; }
            return input.AllResponses.First().Message[12..];
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
            request.AddCommand(CommandsPowerShell.git_Clone, new string[] { "--progress", link, directory});

            var results = await ExecuteAndInformUIAsync(request);

            return results.IsError;
        }

        internal async Task<bool> CreateFolderAsync(string directory, string folderName)
        {
            var request = new PowerShellCommands(2);
            request.AddCommand(CommandsPowerShell.cd, directory);
            request.AddCommand(CommandsPowerShell.md, folderName);

            var results = await ExecuteAndInformUIAsync(request);
            return results.IsError;
        }
    }



}
