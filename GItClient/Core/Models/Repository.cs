using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GItClient.Core.Models
{
    public class RepositoryImage
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public string Path { get; set; }
        public bool Active { get; set; }
        public Color Color { get; set; }

        public RepositoryImage() { }

        public RepositoryImage(Repository repo) 
        {
            Name = repo.Name;
            Link = repo.Link;
            Path = repo.Path;
            Active = repo.Active;
            Color = repo.Color;
        }
    }
    public class Repository 
    {
        public string Name { get; set; }
        public string GenName => Helper.GetGeneratedNameFromPath(Path);
        public string Link { get; set; }
        public string Path { get; set; }
        public bool Active { get; set; }
        public Color Color { get; set; }

        public int CommitsCount { get; set; }

        public GitCommits CommitsHolder { get; set; }

        public Repository() { CommitsHolder = new GitCommits(); Color = Helper.GetRandomColor(); Active = false; }

        public Repository(string path) { Path = path; CommitsHolder = new GitCommits(); Color = Helper.GetRandomColor(); Active = false; }

        public Repository(RepositoryImage repImg) 
        {
            CommitsHolder = new GitCommits();
            Name = repImg.Name;
            Link = repImg.Link;
            Path = repImg.Path;
            Active = repImg.Active;
            Color = repImg.Color;
        }

        //public void RecalculateBranches()
        //{
        //    var result = CommitsHolder.Commits.GroupBy(x => x.Branch);
        //    CommitsByBranchName = result.ToDictionary(x => x.Key, x => x.ToList());

        //    BranchesByName = CommitsByBranchName.Keys.ToDictionary(x => x, x => new Branch(x));
        //}

    }

    public class Branch
    {
        public string Name { get; set; }
        public Color Color { get; set; }

        public Branch(string name) { Name = name;  Color = Helper.GetRandomColor(); }
    }

    




}
