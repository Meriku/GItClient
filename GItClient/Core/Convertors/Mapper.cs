using GItClient.Core.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace GItClient.Core.Convertors
{
    public static class Mapper
    {

        public static TResult Map<TInput, TResult>(TInput input)
        {
            switch (typeof(TResult).Name)
            {
                case nameof(GitCommits):
                    if (input is PowerShellResponses responses)
                    {
                        var result = GetGitCommitsFromPowerShellResponses(responses);
                        if (result is TResult genericResult)
                        {
                            return genericResult;
                        }
                    }
                    break;
            }
            throw new NotImplementedException();
        }

        private static GitCommits GetGitCommitsFromPowerShellResponses(PowerShellResponses responses)
        {
            var responsesArray = responses.AllResponses.ToArray();

            var startIndex = 0;

            var result = new List<GitCommit>();
            while (true)
            {
                var commit = new GitCommit();
                for (var i = startIndex; ; i++)
                {
                    if (i >= responsesArray.Length)
                    {
                        startIndex = i;
                        break;
                    }

                    if (responsesArray[i].Message.Contains("commit"))
                    {
                        if (string.IsNullOrWhiteSpace(commit.CommitHash))
                        {
                            commit.CommitHash = responsesArray[i].Message.Trim();
                        }
                        else
                        {
                            startIndex = i;
                            break;
                        }
                    }
                    else if (responsesArray[i].Message.Contains("Author"))
                    {
                        commit.Author = responsesArray[i].Message.Trim();
                    }
                    else if (responsesArray[i].Message.Contains("Date"))
                    {
                        commit.Date = responsesArray[i].Message.Trim();
                    }
                    else if (DateTime.TryParse(responsesArray[i].Message, out var date))
                    {
                        commit.ShortDate = date;
                    }
                    else
                    {
                        commit.CommitMessage += responsesArray[i].Message.Trim() + " ";
                    }
                }
                result.Add(commit);

                if (startIndex >= responsesArray.Length)
                {
                    break;
                }
            }

            return new GitCommits(result);
        }

    }
}
