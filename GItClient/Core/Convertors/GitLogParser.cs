using GItClient.Core.Models;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace GItClient.Core.Convertors
{
    internal static class GitLogParser
    {
        public const char SEPARATOR = '~';
        public const int HASH_LENGTH = 40;

        public static GitCommits ParseGitGraph(PowerShellResponses responses)
        {
            var PSResponses = responses.AllResponses;

            var result = new GitCommits();
            var commits = new Dictionary<string, GitCommit>();

            foreach (var response in PSResponses)
            {
                var commitInfo = response.Message.Split(SEPARATOR);
                if (commitInfo.Length > 1)
                {
                    var commit = ParseCommit(commitInfo);
                    commits[commit.Hash] = commit;
                }
            }

            result.CommitsMap = commits;
            result.Commits = commits.Values.ToArray();
            
            return result;
        }

        private static GitCommit ParseCommit(string[] commitInfo)
        {
            var commit = new GitCommit();
            try
            {
                commit.Level = ParseCommitLevel(commitInfo[0]);
            }
            catch(Exception e) 
            {
                Console.WriteLine("Exception");
            }
            commit.Hash = commitInfo[1];
            commit.ParentHashes = commitInfo[2].Length > 0 ? commitInfo[2].Split(' ') : Array.Empty<string>();
            commit.AuthorName = commitInfo[3];
            commit.AuthorEmail = commitInfo[4];
            commit.Date = commitInfo[5];
            commit.Subject = commitInfo[6];
            return commit;
        }

        private static int ParseCommitLevel(string line)
        {
            var level = 0;

            for (var i = 0; i < line.Length; i++)
            {
                var letter = line[i];

                if (letter == '*')
                {
                    return level;
                }
                else if (letter == '|')
                {
                    level++;
                }
            }
            throw new Exception("Incorrect commit format");
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
