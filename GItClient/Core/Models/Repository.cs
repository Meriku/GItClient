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
        public string GenName => string.IsNullOrWhiteSpace(Path) ? "" : Path[(Path.LastIndexOf('\\') + 1)..];
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

    }
}
