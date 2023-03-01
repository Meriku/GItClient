using GItClient.Core.Controllers.SettingControllers;
using GItClient.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers
{



    internal static class RepositoriesController
    {
        private static Dictionary<string, Repository> repositories;

        private static RepositoriesSettingsController _repositoriesSettingsController;
        private static GitController _gitController;

        private static string CurrentRepository;

        public static bool IsEmpty => repositories.Count == 0;

        public static void Init()
        {
            _repositoriesSettingsController = new RepositoriesSettingsController();
            _gitController = new GitController();

            Task.Run( async () =>
            {
                repositories = await _repositoriesSettingsController.LoadOpenRepositories();
                StartLoadAllRepositoriesCommits();
            });
        }

        private static async Task LoadRepositoryCommits(Repository repo)
        {
            await repo.CommitsHolder.WaitAsync();
            var commits = await _gitController.GetGitHistoryAsync(repo);
            repo.CommitsHolder.Commits = commits;
            repo.CommitsHolder.Release();
        }

        public static void StartLoadRepositoryCommits(string repoName)
        {
            Task.Run(() => LoadRepositoryCommits(repositories[repoName]));
        }

        public static async Task StartLoadAllRepositoriesCommits()
        {
            foreach (var repo in repositories.Values)
            {
                // TODO: sync for now, issue with powershell which blocks async
                await LoadRepositoryCommits(repo);
            }
        }

        internal static void SetCurrentRepository(string repoName)
        {
            if (string.IsNullOrWhiteSpace(repoName))
            {
                throw new System.Exception("Repository name can't be empty");
            }
            if (!string.IsNullOrWhiteSpace(CurrentRepository))
            {
                repositories[CurrentRepository].Active = false;
            }
            
            CurrentRepository = repoName;
            repositories[CurrentRepository].Active = true;
        }

        internal static Repository GetCurrentRepository()
        {
            if (repositories.Count == 0)
            {
                throw new System.Exception("Repository holder is empty");
            }
            if (CurrentRepository == null)
            {
                return repositories.First().Value;
            }
            return repositories[CurrentRepository];
        }

        internal static Repository GetSpecificRepository(string name)
        {
            if (repositories.ContainsKey(name))
            {
                return repositories[name];
            }
            throw new System.Exception("Repository is absent in repositories");
        }

        internal static void AddRepository(Repository repo)
        {
            if (repositories.ContainsKey(repo.GenName))
            {
                //TODO handle exception, show warning
                throw new System.Exception("Repository is already added to repositories");
            }

            if (!string.IsNullOrWhiteSpace(CurrentRepository))
            {
                repositories[CurrentRepository].Active = false;
            }
            CurrentRepository = repo.GenName;

            repositories[CurrentRepository] = repo;
            repositories[CurrentRepository].Active = true;

            Task.Run( async () => await _repositoriesSettingsController.SaveOpenRepositories(repositories) );

            
        }


        internal static void UpdateRepository(Repository repo)
        {
            if (repositories.ContainsKey(repo.GenName))
            {
                repositories[repo.GenName] = repo;
            }
            else
            {
                throw new System.Exception("Repository is absent in repositories");
            }
            
        }

        internal static Repository[] GetAllOpenRepositories()
        {
            return repositories.Values.ToArray();
        }


    }
}
