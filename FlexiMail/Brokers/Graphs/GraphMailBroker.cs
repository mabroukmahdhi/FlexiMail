// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Threading.Tasks;
using Azure.Identity;
using FlexiMail.Models.Configurations;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace FlexiMail.Brokers.Graphs
{
    internal class GraphMailBroker : IGraphMailBroker
    {
        private readonly GraphServiceClient graphClient;

        public GraphMailBroker(GraphMailConfigurations configurations)
        {
            var credential = new ClientSecretCredential(
                tenantId: configurations.TenantId,
                clientId: configurations.ClientId,
                clientSecret: configurations.ClientSecret);

            this.graphClient = new GraphServiceClient(credential);
        }

        public async ValueTask SendAsync(
            string fromUserIdOrUpn,
            string to,
            string subject,
            string htmlBody,
            bool saveToSentItems = true)
        {
            var message = new Message
            {
                Subject = subject,

                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = htmlBody
                },

                ToRecipients =
                [
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = to
                        }
                    }
                ]
            };

            await this.graphClient.Users[fromUserIdOrUpn]
                .SendMail
                .PostAsync(new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
                {
                    Message = message,
                    SaveToSentItems = saveToSentItems
                });
        }
    }
}
