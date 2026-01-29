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
            var tenantId = Environment.GetEnvironmentVariable("TenantId");
            var authority = $"https://login.microsoftonline.com/{tenantId}";

            var configurations = new ExchangeConfigurations
            {
                ClientId = Environment.GetEnvironmentVariable("ClientId"),
                ClientSecret = Environment.GetEnvironmentVariable("ClientSecret"),
                TenantId = tenantId,
                Authority = authority,
                SmtpAddress = Environment.GetEnvironmentVariable("SmtpAddress"),
                PrincipalName = Environment.GetEnvironmentVariable("PrincipalName"),
                Sid = Environment.GetEnvironmentVariable("Sid"),
                Scopes = ["https://outlook.office365.com/.default"],
            };

            this.flexiMailClient = new FlexiMailClient(configurations);
        }

        private static string GetReceiverTestEmail() =>
            Environment.GetEnvironmentVariable("FlexiTestEmail");
    }
}