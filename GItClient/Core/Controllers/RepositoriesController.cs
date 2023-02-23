using GItClient.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace GItClient.Core.Controllers
{

    

    internal class RepositoriesController
    {
        private Dictionary<string, Repository> repositories;

        private string CurrentRepository;

        public bool IsEmpty => repositories.Count == 0;

        internal RepositoriesController()
        {
            repositories = new Dictionary<string, Repository>();
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
                CurrentRepository = name;
                return repositories[name];
            }
            throw new System.Exception("Repository is absent in repositories");
        }

        internal void AddRepository(Repository repo)
        {
            if (repositories.ContainsKey(repo.GenName))
            {
                throw new System.Exception("Repository is already added to repositories");
            }
            repositories[repo.GenName] = repo;
            CurrentRepository = repo.GenName;
        }

        internal void UpdateRepository(Repository repo)
        {
            if (repositories.ContainsKey(repo.GenName))
            {
                repositories[repo.GenName] = repo;
                CurrentRepository = repo.GenName;
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
