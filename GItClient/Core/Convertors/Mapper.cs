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
        public const char Separator = '~';

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
            var result = new List<GitCommit>();

            foreach (var response in responsesArray)
            {
                var commit = new GitCommit();
                commit.CommitHash = response.Message[0..40];

                var subjectEndIndex = response.Message.IndexOf(Separator, 41);
                //var safeSubjectEndIndex = subjectEndIndex == -1 ? 41 : subjectEndIndex;
                commit.Subject = response.Message[41..subjectEndIndex];

                var bodyEndIndex = response.Message.IndexOf(Separator, subjectEndIndex + 1);
                commit.Body = response.Message[(subjectEndIndex + 1)..bodyEndIndex];

                var authorEndIndex = response.Message.IndexOf(Separator, bodyEndIndex + 1);
                commit.Author = response.Message[(bodyEndIndex + 1)..authorEndIndex];

                var emailEndIndex = response.Message.IndexOf(Separator, authorEndIndex + 1);
                commit.Email = response.Message[(authorEndIndex + 1)..emailEndIndex];

                commit.Date = response.Message[(emailEndIndex + 1)..];
                
                if (DateTime.TryParse(commit.Date, out var shortDate))
                {
                    commit.ShortDate = shortDate;
                }
                
                result.Add(commit);
            }

            return new GitCommits(result);
        }

    }
}
