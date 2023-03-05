using GItClient.Core.Controllers.Static;
using GItClient.Core.Convertors;
using GItClient.Core.Models;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers
{
    /// <summary>
    /// Controller 
    /// Provides "API" for Git commands
    /// </summary>
    internal class GitController
    {
        private async Task<PowerShellResponses> ExecuteAndInformUIAsync(PowerShellCommands request)
        {
            return await MainGitController.ExecuteAndInformUIAsync(request);
        }

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
            request.AddCommand(CommandsPowerShell.git_Log, new string[] { "--pretty=%H¦%s¦%b¦%aN¦%aE¦%aD" });

            var results = await ExecuteAndInformUIAsync(request);

            var result = Mapper.Map<PowerShellResponses, GitCommits>(results);

            return result.Commits;
        }

        internal async Task<bool> CheckIfRepositoryExist(string directory)
        {
            var request = new PowerShellCommands(2, internalUsage: true);
            request.AddCommand(CommandsPowerShell.cd, directory);
            request.AddCommand(CommandsPowerShell.git_Revparse, "--git-dir");

            var results = await ExecuteAndInformUIAsync(request);

            return results.AllResponses.Any(r => r.Message.Equals(".git"));
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
            request.AddCommand(CommandsPowerShell.git_Clone, new string[] { "--recurse-submodules", link, directory, "--verbose"});

            var results = await ExecuteAndInformUIAsync(request);

            // TODO: output somehow goes to ERROR stream
            //if (!results.IsError)
            //{
            //    var repo = new Repository(directory);
            //    RepositoriesController.AddRepository(repo);
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
