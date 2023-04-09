using GItClient.Core.Models;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Convertors
{
    internal static class GitLogParser
    {
        public const char SEPARATOR = '~';
        public const int HASH_LENGTH = 40;

        public static GitCommits ConvertPSResponsesToGitCommits(PowerShellResponses responses)
        {
            var PSResponses = responses.AllResponses.ToArray();
            var result = new List<GitCommit>();

            var tempCommit = new GitCommit();
            for (var i = 0; i < PSResponses.Length; i++)
            {
                if (PSResponses[i].Message.Length == HASH_LENGTH)
                {
                    tempCommit = new GitCommit() { Hash = PSResponses[i].Message };
                    result.Add(tempCommit);
                }
                else if (PSResponses[i].Message.StartsWith("branch :"))
                {
                    tempCommit.Branch = PSResponses[i].Message[8..];
                }
                else
                {
                    var commitBodyArray = PSResponses[i].Message.Split(SEPARATOR);
                    tempCommit.ParentHashes = commitBodyArray[0].Length > 0 ? commitBodyArray[0].Split(' ') : Array.Empty<string>();
                    tempCommit.AuthorName = commitBodyArray[1];
                    tempCommit.AuthorEmail = commitBodyArray[2];
                    tempCommit.Date = commitBodyArray[3];
                    tempCommit.Subject = commitBodyArray[4];
                    tempCommit.Body = commitBodyArray[5];
                }        
            }

            return new GitCommits(result);

        }

        public static CommitsTree CreateTree(Repository repository)
        {
            var commits = repository.CommitsHolder.Commits;
            var tree = new CommitsTree();

            for (var i = commits.Length - 1; i >= 0; i--)
            {
                tree.Add(commits[i]);
            }

            return tree;

        }

    }
}
