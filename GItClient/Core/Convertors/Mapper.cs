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

            try
            {
                for (var r = 0; r < responsesArray.Length; r++)
                {
                    var response = responsesArray[r];
                    var commit = new GitCommit();

                    //TODO: branhes don't work correctly right now. Fix
                    if (response.Message.Contains("branch :"))
                    {
                        commit.Branch = response.Message[8..];
                        r++;
                        response = responsesArray[r];
                        var responseSplited = response.Message.Split(GitLogParser.Separator);
                        commit.Author = responseSplited[1];
                        commit.Email = responseSplited[2];
                        commit.Date = responseSplited[3];
                        if (DateTime.TryParse(responseSplited[3], out var shortDate))
                        {
                            commit.ShortDate = shortDate;
                        }
                        result.Add(commit);
                        commit = new GitCommit();
                        r++;
                        response = responsesArray[r];
                    }

                    

                    var indexes = new List<int>() { 0 };
                    var startPoint = 0;
                    while (true)
                    {
                        if (startPoint > response.Message.Length) { break; }

                        var index = response.Message.IndexOf(GitLogParser.Separator, startPoint);
                        startPoint = index + 1;

                        if (index == -1) { break; }

                        indexes.Add(index);
                    }

                    for (var i = 0; i < indexes.Count; i++)
                    {
                        var message = String.Empty;
                        if (i + 1 == indexes.Count)
                        {
                            message = response.Message[(indexes[i] + 1)..];
                        }
                        else
                        {
                            if ((indexes[i] + 1) != indexes[i + 1])
                            {
                                message = response.Message[(indexes[i] + 1)..indexes[i + 1]];
                            }
                        }

                        switch (i)
                        {
                            case 0:
                                commit.CommitHash = message;
                                break;
                            case 1:
                                commit.Subject = message;
                                break;
                            case 2:
                                commit.Body = message;
                                break;
                            case 3:
                                commit.Author = message;
                                break;
                            case 4:
                                commit.Email = message;
                                break;
                            case 5:
                                commit.Date = message;
                                if (DateTime.TryParse(message, out var shortDate))
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
