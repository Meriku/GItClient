using GItClient.Core.Base;
using GItClient.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers
{

    

    internal class RepositoriesController : SettingsBase<RepositorySettings>
    {
        private Dictionary<string, Repository> repositories;

        private string CurrentRepository;
        private string PreviousRepository;

        public bool IsEmpty => repositories.Count == 0;



        internal RepositoriesController()
        {
            repositories = new Dictionary<string, Repository>();

            LoadOpenRepositories();
        }


        private async void LoadOpenRepositories()
        {
            var RepositoriesImage = await base.GetSpecificSetting();

            if (RepositoriesImage.ActiveRepositories.Length > 0)
            {
                var Repositories = RepositoriesImage.ActiveRepositories.Select(x => new Repository(x));

                repositories = Repositories.ToDictionary(k => k.GenName, v => v);
            }

            var ActiveRepo = repositories.Values.FirstOrDefault(x => x.Active);

            if (ActiveRepo == null)
            {
                //TODO: logger
                throw new System.Exception("Failed to load repositories from the last session.");
            }

            //TODO: handle if loaded with 0 repositories (it's possible)
            //TODO: load commits here?
            CurrentRepository = ActiveRepo == null ? "" : ActiveRepo.GenName;
        }

        private async Task SaveRepositories()
        {
            var repositoriesImage = repositories.Values.Select(x => new RepositoryImage(x)).ToArray();

            var Setting = new RepositorySettings(repositoriesImage);

            await base.SetSpecificSetting(Setting);
        }


        private async Task LoadRepositoryCommits(string repoName)
        {
            var repo = repositories[repoName];

            var _gitController = ControllersProvider.GetGitController();
            await repo.CommitsHolder.WaitAsync();
            var commits = await _gitController.GetGitHistoryAsync(repo);
            repo.CommitsHolder.Commits = commits;
            repo.CommitsHolder.Release();
        }

        public void StartLoadRepositoryCommits(string repoName)
        {
            Task.Run(() => LoadRepositoryCommits(repoName));
        }

        internal void SetCurrentRepository(string repoName)
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

        internal Repository GetCurrentRepository()
        {
            if (repositories.Count == 0)
            {
                throw new System.Exception("Repository holder is empty");
            }
            return repositories[CurrentRepository];
        }

        internal Repository GetSpecificRepository(string name)
        {
            if (repositories.ContainsKey(name))
            {
                return repositories[name];
            }
            throw new System.Exception("Repository is absent in repositories");
        }

        internal void AddRepository(Repository repo)
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

            Task.Run( () => SaveRepositories() );

            
        }


        internal void UpdateRepository(Repository repo)
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

        internal Repository[] GetAllOpenRepositories()
        {
            return repositories.Values.ToArray();
        }


    }
}
