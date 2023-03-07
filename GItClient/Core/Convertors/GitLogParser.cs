using GItClient.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Convertors
{
    internal static class GitLogParser
    {
        public const char Separator = '~';
        public static Tree<GitCommitBase> CreateTree(PowerShellResponses responses)
        {
            // Each line in responses contain hash and parent hash (if first commit - only hash)

            var lines = responses.AllResponses.Select(x => x.Message).ToArray();
            var tree = new Tree<GitCommitBase>();

            for (var i = lines.Length - 1; i >= 0; i--)
            {
                var line = lines[i];

                var hash = line[0..40];
                var parentHashesArray = Array.Empty<string>();

                if (line.Length > 45)
                {
                    var parentHashes = line.Split(Separator)[1];
                    parentHashesArray = parentHashes.Split(' ');
                }

                var commit = new GitCommitBase(hash, parentHashesArray);
                tree.Add(commit);

            }

            return tree;

        }



    }
}
