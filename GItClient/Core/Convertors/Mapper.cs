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
                        r++;

                        //response = responsesArray[r];
                        //var responseSplited = response.Message.Split(GitLogParser.Separator);
                        //commit.Author = responseSplited[1];
                        //commit.Email = responseSplited[2];
                        //commit.Date = responseSplited[3];
                        //if (DateTime.TryParse(responseSplited[3], out var shortDate))
                        //{
                        //    commit.ShortDate = shortDate;
                        //}
                        //result.Add(commit);

                        
                        r++;
                        commit = new GitCommit();
                        response = responsesArray[r];
                    }
                    else
                    {
                        currentBranch = "master";
                    }

                   
                    var strings = response.Message.Split(GitLogParser.Separator);
                    commit.Branch = currentBranch;
                    for (var i = 0; i < strings.Length; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                commit.CommitHash = strings[i];
                                break;
                            case 1:
                                commit.Subject = strings[i];
                                break;
                            case 2:
                                commit.Body = strings[i];
                                break;
                            case 3:
                                commit.Author = strings[i];
                                break;
                            case 4:
                                commit.Email = strings[i];
                                break;
                            case 5:
                                commit.Date = strings[i];
                                if (DateTime.TryParse(strings[i], out var shortDate))
                                {
                                    commit.ShortDate = shortDate;
                                }
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
