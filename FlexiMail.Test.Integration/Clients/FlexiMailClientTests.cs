// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System;
using FlexiMail.Models.Configurations;

namespace FlexiMail.Test.Integration.Clients
{
    public partial class FlexiMailClientTests
    {
        private readonly IFlexiMailClient flexiMailClient;

        public FlexiMailClientTests()
        {
            var configurations = new GraphMailConfigurations
            {
                ClientId = Environment.GetEnvironmentVariable("ClientId"),
                ClientSecret = Environment.GetEnvironmentVariable("ClientSecret"),
                TenantId = Environment.GetEnvironmentVariable("TenantId"),
                SenderUserIdOrUpn = Environment.GetEnvironmentVariable("SmtpAddress"),
                Scopes = ["https://graph.microsoft.com/.default"],
            };

            this.flexiMailClient = new FlexiMailClient(configurations);
        }

        private static string GetReceiverTestEmail() =>
            Environment.GetEnvironmentVariable("FlexiTestEmail");
    }
}