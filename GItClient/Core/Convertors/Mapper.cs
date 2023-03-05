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
            var result = new List<GitCommit>();
            //TODO: solve another encoding problem

            foreach (var response in responsesArray)
            {
                var commit = new GitCommit();
                commit.CommitHash = response.Message[0..40];

                var subjectEndIndex = response.Message.IndexOf('¦', 41);
                commit.Subject = response.Message[41..subjectEndIndex];

                var bodyEndIndex = response.Message.IndexOf('¦', subjectEndIndex + 1);
                commit.Body = response.Message[(subjectEndIndex + 1)..bodyEndIndex];

                var authorEndIndex = response.Message.IndexOf('¦', bodyEndIndex + 1);
                commit.Author = response.Message[(bodyEndIndex + 1)..authorEndIndex];

                var emailEndIndex = response.Message.IndexOf('¦', authorEndIndex + 1);
                commit.Email = response.Message[(authorEndIndex + 1)..emailEndIndex];

                commit.Date = response.Message[(emailEndIndex + 1)..];
            
                result.Add(commit);
            }

            return new GitCommits(result);
        }

    }
}
