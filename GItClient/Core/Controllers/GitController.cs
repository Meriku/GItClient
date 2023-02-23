using GItClient.Core.Base;
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
        protected string? _gitVersion;
        internal List<Repository> _openRepositories = new List<Repository>();

        private RepositoriesController _repositoriesController;

        internal GitController()
        {
            _repositoriesController = ControllersProvider.GetRepositoriesController();
        }

        internal async Task<string> GetGitVersionAsync()
        {
            if (_gitVersion != null) return _gitVersion;

            var request = new PowerShellCommands();
            request.AddCommand(CommandsPowerShell.git_Version);

            var results = await ExecuteAndInformUIAsync(request);

            _gitVersion = ParseVersion(results); 

            return _gitVersion;  
        }
        internal async Task<GitCommits> GetGitHistoryAsync(Repository repository)
        {
            var request = new PowerShellCommands(2, internalUsage: true);
            request.AddCommand(CommandsPowerShell.cd, repository.Path);
            request.AddCommand(CommandsPowerShell.git_Log, "--decorate=short");

            var results = await ExecuteAndInformUIAsync(request);

            var result = Mapper.Map<PowerShellResponses, GitCommits>(results);

            return result;
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
                AddRepositoryToController(directory);
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
            AddRepositoryToController(directory);

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
            AddRepositoryToController(directory);

            return true;
        }

        private void AddRepositoryToController(string directory)
        {
            Task.Run( async () => 
            {
                var repo = new Repository();
                repo.Path = directory;
                _repositoriesController.AddRepository(repo);

                repo.CommitsHolder.IsLoading = true;

                //TODO: try catch? 
                await repo.CommitsHolder.semaphore.WaitAsync();
                var commits = await GetGitHistoryAsync(repo);

                Thread.Sleep(10000);
                repo.CommitsHolder.Commits = commits.Commits;
                repo.CommitsHolder.semaphore.Release();

                repo.CommitsHolder.IsLoading = false;
                _repositoriesController.UpdateRepository(repo);

            });
        }

    }



}
