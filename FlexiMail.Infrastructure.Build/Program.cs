//---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
//---------------------------------------

using System.Collections.Generic;
using System.IO;
using ADotNet.Clients;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV3s;

namespace FlexiMail.Infrastructure.Build
{
    internal class Program
    {
        private const string BuildScriptPath = "../../../../.github/workflows/dotnet.yml";

        private static void Main(string[] args)
        {
            var adoNetClient = new ADotNetClient();

            var githubPipeline = new GithubPipeline
            {
                Name = "FlexiMail Build",

                OnEvents = new Events
                {
                    Push = new PushEvent
                    {
                        Branches = new string[] { "main" }
                    },

                    PullRequest = new PullRequestEvent
                    {
                        Branches = new string[] { "main" }
                    }
                },

                Jobs = new Dictionary<string, Job>
                {
                    {
                        "build",
                        new Job
                        {
                            EnvironmentVariables = new Dictionary<string, string>
                            {
                                { "Authority", "${{ secrets.AUTHORITY }}" },
                                { "ClientId", "${{ secrets.CLIENTID }}" },
                                { "ClientSecret", "${{ secrets.CLIENTSECRET }}" },
                                { "PrincipalName", "${{ secrets.PRINCIPALNAME }}" },
                                { "Sid", "${{ secrets.SID }}" },
                                { "SmtpAddress", "${{ secrets.SMTPADDRESS }}" },
                                { "TenantId", "${{ secrets.TENANTID }}" },
                                { "FlexiTestEmail", "${{ secrets.FLEXITESTEMAIL }}" },
                            },

                            RunsOn = BuildMachines.WindowsLatest,

                            Steps =
                            [
                                new CheckoutTaskV3
                                {
                                    Name = "Pulling Code"
                                },


                                new SetupDotNetTaskV3
                                {
                                    Name = "Installing .NET",

                                    With = new TargetDotNetVersionV3
                                    {
                                        DotNetVersion = "8.0.*"
                                    }
                                },


                                new RestoreTask
                                {
                                    Name = "Restoring Packages"
                                },


                                new DotNetBuildTask
                                {
                                    Name = "Building Solution"
                                },


                                new TestTask
                                {
                                    Name = "Running Tests"
                                }
                            ]
                        }
                    }
                }
            };

            var directoryPath = Path.GetDirectoryName(BuildScriptPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath!);
            }

            adoNetClient.SerializeAndWriteToFile(
                adoPipeline: githubPipeline,
                path: BuildScriptPath);
        }
    }
}