using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GItClient.Core.Models
{

    public class GitCommitBase
    {
        public string Hash { get; set; }
        public string[] ParentHashes { get; set; }

        public GitCommitBase(string hash, string[] parentHashes) 
        {
            Hash = hash;
            ParentHashes = parentHashes;
        }

        public GitCommitBase() { }
    }

    public class GitCommitExtended : GitCommitBase
    {
        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public string Date { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Branch { get; set; }

        public GitCommitExtended() { }
    }

    public class GitCommit : GitCommitExtended
    {
        public string ShortHash => string.IsNullOrWhiteSpace(Hash) ? "" : Hash[0..7];
        public DateTime ShortDate => DateTime.TryParse(Date, out var shortDate) ? shortDate : DateTime.MinValue;
        public string ShortDateString => ShortDate.ToString("g");

        public GitCommit() { }
    }
    public class GitCommits : IEnumerable<GitCommit>
    {
        public GitCommit[] Commits { get; set; }
        public int Lenght => Commits.Length;

        public bool IsError { get; set; }
        public bool IsLoading => semaphore.CurrentCount == 0;
        public bool IsEmpty => Commits.Length == 0;

        private readonly SemaphoreSlim semaphore;

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

        public async Task WaitAsync()
        {
            await semaphore.WaitAsync();
        }
        public void Release()
        {
            semaphore.Release();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
