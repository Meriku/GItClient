using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GItClient.Core.Models
{
    public class GitCommit
    {
        public string CommitHash { get; set; }
        public string ShortCommitHash => string.IsNullOrWhiteSpace(CommitHash) ? "" : CommitHash[7..13];
        public string Author { get; set; }
        public string Date { get; set; }
        public string Empty1 { get; set; }
        public DateTime ShortDate { get; set; }
        public string CommitMessage { get; set; }
        public string Empty2 { get; set; }

        public GitCommit()
        {

        }
    }
    public class GitCommits : IEnumerable<GitCommit>
    {
        public GitCommit[] Commits { get; set; }
        public int Lenght => Commits.Length;

        public bool IsError { get; set; }
        public bool IsLoading { get; set; }
        public bool IsEmpty => Commits.Length == 0;

        public readonly SemaphoreSlim semaphore;

        public GitCommits(List<GitCommit> commits)
        {
            Commits = commits.ToArray();
            semaphore = new SemaphoreSlim(1);
        }
        public GitCommits()
        {
            Commits = Array.Empty<GitCommit>();
            semaphore = new SemaphoreSlim(1);
        }

        public IEnumerator<GitCommit> GetEnumerator()
        {
            foreach (var commit in Commits)
            {
                yield return commit;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
