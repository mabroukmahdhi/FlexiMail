// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------
#if !NET10_0
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlexiMail.Models.Configurations;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Identity.Client;

namespace FlexiMail.Brokers.Exchanges
{
    internal class ExchangeBroker(ExchangeConfigurations configurations) : IExchangeBroker
    {
        private const string ExchangeServiceUrl = "https://outlook.office365.com/EWS/Exchange.asmx";

        private readonly IConfidentialClientApplication clientApplication =
            ConfidentialClientApplicationBuilder.Create(configurations.ClientId)
                .WithClientSecret(configurations.ClientSecret)
                .WithAuthority(new Uri(configurations.Authority))
                .Build();

        public async ValueTask<string> GetAccessTokenAsync()
        {
            var result = await clientApplication.AcquireTokenForClient(configurations.Scopes)
                .ExecuteAsync();

            return result.AccessToken;
        }

        public ExchangeService CreateExchangeService(ExchangeVersion version, string accessToken,
            ImpersonatedUserId impersonatedUserId)
        {
            return new ExchangeService(version)
            {
                Credentials = new OAuthCredentials(accessToken),
                Url = new Uri(ExchangeServiceUrl),
                ImpersonatedUserId = impersonatedUserId
            };
        }

        public void Reply(EmailMessage emailMessage, MessageBody bodyPrefix, bool replyAll) =>
            emailMessage.Reply(bodyPrefix, replyAll);

        public void Forward(EmailMessage emailMessage, MessageBody bodyPrefix, params EmailAddress[] toRecipients) =>
            emailMessage.Forward(bodyPrefix, toRecipients);

        public void Forward(EmailMessage emailMessage, MessageBody bodyPrefix,
            IEnumerable<EmailAddress> toRecipients) =>
            emailMessage.Forward(bodyPrefix, toRecipients);

        public void Send(EmailMessage emailMessage) =>
            emailMessage.Send();

        public void SendAndSaveCopy(EmailMessage emailMessage, FolderId destinationFolderId) =>
            emailMessage.SendAndSaveCopy(destinationFolderId);

        public void SendAndSaveCopy(EmailMessage emailMessage) =>
            emailMessage.SendAndSaveCopy();

        public void SuppressReadReceipt(EmailMessage emailMessage) =>
            emailMessage.SuppressReadReceipt();
    }
}
#endif