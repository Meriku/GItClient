using GItClient.Core.Models;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GItClient.Core.Convertors
{
    internal static class GitLogParser
    {
        public const char SEPARATOR = '~';
        public const int HASH_LENGTH = 40;
        public const string EMPTY = " ";

        public static GitCommits ConvertPSResponsesToGitCommits(PowerShellResponses responses)
        {
            var PSResponses = responses.AllResponses.ToArray();
            var result = new List<GitCommit>();

            var tempCommit = new GitCommit();
            for (var i = 0; i < PSResponses.Length; i++)
            {
                if (PSResponses[i].Message.Length == HASH_LENGTH)
                {
                    var regex = new Regex("^[0-9a-fA-F]{40}$");
                    if (regex.IsMatch(PSResponses[i].Message))
                    {
                        tempCommit = new GitCommit() { Hash = PSResponses[i].Message };
                        result.Add(tempCommit);
                    }
                }
                else if (PSResponses[i].Message.StartsWith("branch :"))
                {
                    tempCommit.Branch = PSResponses[i].Message[8..];
                }
                else if (PSResponses[i].Message[0] == SEPARATOR)
                {
                    var commitBodyArray = PSResponses[i].Message[1..].Split(SEPARATOR);
                    tempCommit.ParentHashes = commitBodyArray[0].Length > 0 ? commitBodyArray[0].Split(' ') : Array.Empty<string>();
                    tempCommit.AuthorName = commitBodyArray.Length > 1 ? commitBodyArray[1] : EMPTY;
                    tempCommit.AuthorEmail = commitBodyArray.Length > 2 ? commitBodyArray[2] : EMPTY;
                    tempCommit.Date = commitBodyArray.Length > 3 ? commitBodyArray[3] : EMPTY;
                    tempCommit.Subject = commitBodyArray.Length > 4 ? commitBodyArray[4] : EMPTY;
                    tempCommit.Body = commitBodyArray.Length > 5 ? commitBodyArray[5] : EMPTY;
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
