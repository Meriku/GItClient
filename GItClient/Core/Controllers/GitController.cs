using GItClient.Core.Controllers.Base;
using GItClient.Core.Controllers.Static;
using GItClient.Core.Convertors;
using GItClient.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace GItClient.Core.Controllers
{
    /// <summary>
    /// Controller 
    /// Provides "API" for Git commands
    /// </summary>
    internal class GitController : GitControllerBase
    {

        internal async Task<string> GetGitVersionAsync()
        {
            var request = new PowerShellCommands();
            request.AddCommand(CommandsPowerShell.git_Version);

            var results = await ExecuteAndInformUIAsync(request);

            var gitVersion = ParseVersion(results); 

            return gitVersion;  
        }
        internal async Task<GitCommit[]> GetGitHistoryAsync(Repository repository)
        {
            var request = new PowerShellCommands(2, internalUsage: true);
            request.AddCommand(CommandsPowerShell.cd, repository.Path);
            request.AddCommand(CommandsPowerShell.git_Log, "--decorate=short");

            var results = await ExecuteAndInformUIAsync(request);

            var result = Mapper.Map<PowerShellResponses, GitCommits>(results);

            return result.Commits;
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

            if (!results.IsError)
            {
                var repo = new Repository(directory);
                RepositoriesController.AddRepository(repo);
            }

            return results.IsError;         
        }

        internal async Task<bool> CloneRepositoryAsync(string directory, string link)
        {
            var request = new PowerShellCommands();
            request.AddCommand(CommandsPowerShell.git_Clone, new string[] { "--progress", link, directory});

            var results = await ExecuteAndInformUIAsync(request);

            // TODO: Somehow cloning stream goes to Error Stream
            //if (!results.IsError)
            //{
            //    AddRepositoryToController(directory);
            //}

            var repo = new Repository(directory);
            RepositoriesController.AddRepository(repo);

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

        internal bool OpenRepository(string directory)
        {
            var repo = new Repository(directory);
            RepositoriesController.AddRepository(repo);

            return true;
        }

    }



}
