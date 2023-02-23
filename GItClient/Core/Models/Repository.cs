using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Models
{
    public class Repository
    {
        // TODO: just MVP

        public string Name { get; set; }
        public string GenName => string.IsNullOrWhiteSpace(Path) ? "" : Path[(Path.LastIndexOf('\\') + 1)..];
        public string Link { get; set; }
        public string Path { get; set; }
        public int CommitsCount { get; set; }

        public GitCommits CommitsHolder { get; set; }

        public Repository() { CommitsHolder = new GitCommits(); }

        public Repository(string path) { Path = path; CommitsHolder = new GitCommits(); }

    }
}
