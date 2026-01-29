// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Attachments;
using FlexiMail.Models.Foundations.Bodies;
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
            IEnumerable<string> toRecipients,
            IEnumerable<string> ccRecipients,
            IEnumerable<string> bccRecipients,
            string subject,
            string body,
            BodyContentType bodyContentType,
            IEnumerable<FlexiAttachment> attachments,
            bool saveToSentItems = true)
        {
            var message = new Message
            {
                Subject = subject,

                Body = new ItemBody
                {
                    ContentType = MapBodyType(bodyContentType),
                    Content = body
                },

                ToRecipients = MapRecipients(toRecipients),
                CcRecipients = MapRecipients(ccRecipients),
                BccRecipients = MapRecipients(bccRecipients),
                Attachments = MapAttachments(attachments)
            };

            await this.graphClient.Users[fromUserIdOrUpn]
                .SendMail
                .PostAsync(new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
                {
                    Message = message,
                    SaveToSentItems = saveToSentItems
                });
        }

        private static BodyType MapBodyType(BodyContentType bodyContentType) => bodyContentType switch
        {
            BodyContentType.Html => BodyType.Html,
            BodyContentType.PlainText => BodyType.Text,
            _ => BodyType.Html
        };

        private static List<Recipient> MapRecipients(IEnumerable<string> recipients)
        {
            if (recipients is null)
            {
                return null;
            }

            var mappedRecipients = recipients
                .Where(recipient => !string.IsNullOrWhiteSpace(recipient))
                .Select(recipient => new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = recipient
                    }
                })
                .ToList();

            return mappedRecipients.Count > 0 ? mappedRecipients : null;
        }

        private static List<Attachment> MapAttachments(IEnumerable<FlexiAttachment> attachments)
        {
            if (attachments is null)
            {
                return null;
            }

            var mappedAttachments = attachments
                .Where(attachment => attachment != null)
                .Select(attachment => new FileAttachment
                {
                    Name = attachment.Name,
                    ContentBytes = attachment.Bytes
                } as Attachment)
                .ToList();

            return mappedAttachments.Count > 0 ? mappedAttachments : null;
        }
    }
}
