using GItClient.Core.Base;
using GItClient.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GItClient.Core.Controllers.SettingControllers
{
    internal class RepositoriesSettingsController : SettingsBase<RepositorySettings>
    {
        internal async Task SaveOpenRepositories(Dictionary<string, Repository> repositories)
        {
            var repositoriesImage = repositories.Values.Select(x => new RepositoryImage(x)).ToArray();

            var Setting = new RepositorySettings(repositoriesImage);

            await SetSpecificSetting(Setting);
        }

        internal async Task<Dictionary<string, Repository>> LoadOpenRepositories()
        {
            var repositories = new Dictionary<string, Repository>();

            var repositoriesImage = await GetSpecificSetting();

            if (repositoriesImage.ActiveRepositories.Length > 0)
            {
                var repositoriesArray = repositoriesImage.ActiveRepositories.Select(x => new Repository(x));

                repositories = repositoriesArray.ToDictionary(k => k.GenName, v => v);
            }

            return repositories;

            //var ActiveRepo = repositories.Values.FirstOrDefault(x => x.Active);

            //if (ActiveRepo == null)
            //{
            //    //TODO: logger
            //    throw new System.Exception("Failed to load repositories from the last session.");
            //}

            //TODO: handle if loaded with 0 repositories (it's possible)
            //TODO: load commits here?
            //CurrentRepository = ActiveRepo == null ? "" : ActiveRepo.GenName;

            //foreach (var repository in repositories.Values)
            //{
            //    StartLoadRepositoryCommits(repository.GenName);
            //}
        }
    }
}
