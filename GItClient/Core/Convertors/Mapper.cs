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

            var currentBranch = "master";
            try
            {
                for (var r = 0; r < responsesArray.Length; r++)
                {
                    var response = responsesArray[r];
                    var commit = new GitCommit();

                    //TODO: branhes don't work correctly right now. Fix
                    if (response.Message.Contains("branch :"))
                    {
                        currentBranch = response.Message[8..];
                    }

                    //if (response.Message.Contains("Merge branch"))
                    //{
                    //    commit.Branch = currentBranch;
                    //    var stringsMerge = response.Message.Split(GitLogParser.Separator);

                    //    for (var i = 0; i < stringsMerge.Length; i++)
                    //    {
                    //        switch (i)
                    //        {
                    //            case 0:
                    //                commit.CommitHash = stringsMerge[i];
                    //                break;
                    //            case 1:
                    //                commit.Subject = stringsMerge[i];
                    //                break;
                    //            case 2:
                    //                commit.Body = stringsMerge[i];
                    //                break;
                    //            case 3:
                    //                commit.Author = stringsMerge[i];
                    //                break;
                    //            case 4:
                    //                commit.Email = stringsMerge[i];
                    //                break;
                    //            case 5:
                    //                commit.Date = stringsMerge[i];
                    //                if (DateTime.TryParse(stringsMerge[i], out var shortDate))
                    //                {
                    //                    commit.ShortDate = shortDate;
                    //                }
                    //                break;
                    //        }
                    //    }
                    //}

                    var strings = response.Message.Split(GitLogParser.SEPARATOR);
                    if (strings[0].Length != 40) // 40 - standart hash length
                    {
                        continue;
                    }
                    commit.Branch = currentBranch;
                    for (var i = 0; i < strings.Length; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                commit.Hash = strings[i];
                                break;
                            case 1:
                                commit.Subject = strings[i];
                                break;
                            case 2:
                                commit.Body = strings[i];
                                break;
                            case 3:
                                commit.AuthorName = strings[i];
                                break;
                            case 4:
                                commit.AuthorEmail = strings[i];
                                break;
                        }
                    }

                    result.Add(commit);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }


            return new GitCommits(result);
        }


        

    }
}
