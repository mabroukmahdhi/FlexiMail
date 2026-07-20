// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Attachments;
using FlexiMail.Models.Foundations.Bodies;
using FlexiMail.Models.Foundations.Inbounds;
using FlexiMail.Models.Foundations.Subscriptions;
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
            var mappedAttachments = MapAttachments(attachments);
            var mappedToRecipients = MapRecipients(toRecipients);
            var mappedCcRecipients = MapRecipients(ccRecipients);
            var mappedBccRecipients = MapRecipients(bccRecipients);

            var message = new Message
            {
                Subject = subject,

                Body = new ItemBody
                {
                    ContentType = MapBodyType(bodyContentType),
                    Content = body
                }
            };

            if (mappedToRecipients != null)
            {
                message.ToRecipients = mappedToRecipients;
            }

            if (mappedCcRecipients != null)
            {
                message.CcRecipients = mappedCcRecipients;
            }

            if (mappedBccRecipients != null)
            {
                message.BccRecipients = mappedBccRecipients;
            }

            if (mappedAttachments != null)
            {
                message.Attachments = mappedAttachments;
            }

            await this.graphClient.Users[fromUserIdOrUpn]
                .SendMail
                .PostAsync(new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
                {
                    Message = message,
                    SaveToSentItems = saveToSentItems
                });
        }

        public async ValueTask<FlexiReceivedMessagePage> GetInboxAsync(
            string mailbox,
            int pageSize,
            bool unreadOnly,
            CancellationToken cancellationToken)
        {
            var response = await this.graphClient.Users[mailbox]
                .MailFolders["inbox"]
                .Messages
                .GetAsync(request =>
                {
                    request.QueryParameters.Top = pageSize;
                    request.QueryParameters.Select = MessageProperties;

                    if (unreadOnly)
                    {
                        request.QueryParameters.Filter = "isRead eq false";
                    }
                    else
                    {
                        request.QueryParameters.Orderby = ["receivedDateTime desc"];
                    }
                }, cancellationToken);

            return new FlexiReceivedMessagePage
            {
                Messages = response?.Value?.Select(MapMessage).ToList() ?? [],
                NextLink = response?.OdataNextLink
            };
        }

        public async ValueTask<FlexiReceivedMessage> GetMessageAsync(
            string mailbox,
            string messageId,
            CancellationToken cancellationToken)
        {
            var message = await this.graphClient.Users[mailbox]
                .Messages[messageId]
                .GetAsync(request =>
                {
                    request.QueryParameters.Select = MessageProperties;
                    request.QueryParameters.Expand = ["attachments"];
                }, cancellationToken);

            return MapMessage(message);
        }

        public async ValueTask<FlexiMailSubscription> CreateInboxSubscriptionAsync(
            string mailbox,
            string notificationUrl,
            string lifecycleNotificationUrl,
            string clientState,
            CancellationToken cancellationToken)
        {
            var subscription = await this.graphClient.Subscriptions.PostAsync(
                new Subscription
                {
                    ChangeType = "created",
                    NotificationUrl = notificationUrl,
                    LifecycleNotificationUrl = lifecycleNotificationUrl,
                    Resource = $"users/{mailbox}/mailFolders('inbox')/messages",
                    ClientState = clientState,
                    ExpirationDateTime = DateTimeOffset.UtcNow.AddDays(6)
                }, cancellationToken: cancellationToken);

            return MapSubscription(subscription);
        }

        public async ValueTask<FlexiMailSubscription> RenewSubscriptionAsync(
            string subscriptionId,
            CancellationToken cancellationToken)
        {
            var subscription = await this.graphClient.Subscriptions[subscriptionId]
                .PatchAsync(new Subscription
                {
                    ExpirationDateTime = DateTimeOffset.UtcNow.AddDays(6)
                }, cancellationToken: cancellationToken);

            return MapSubscription(subscription);
        }

        public async ValueTask DeleteSubscriptionAsync(
            string subscriptionId,
            CancellationToken cancellationToken) =>
            await this.graphClient.Subscriptions[subscriptionId]
                .DeleteAsync(cancellationToken: cancellationToken);

        private static readonly string[] MessageProperties =
        [
            "id", "internetMessageId", "conversationId", "from", "toRecipients",
            "ccRecipients", "subject", "body", "bodyPreview", "receivedDateTime",
            "isRead", "hasAttachments", "webLink"
        ];

        private static FlexiReceivedMessage MapMessage(Message message)
        {
            if (message is null)
            {
                return null;
            }

            return new FlexiReceivedMessage
            {
                Id = message.Id,
                InternetMessageId = message.InternetMessageId,
                ConversationId = message.ConversationId,
                From = message.From?.EmailAddress?.Address,
                To = MapAddresses(message.ToRecipients),
                Cc = MapAddresses(message.CcRecipients),
                Subject = message.Subject,
                Body = message.Body is null ? null : new FlexiBody
                {
                    Content = message.Body.Content,
                    ContentType = message.Body.ContentType == BodyType.Text
                        ? BodyContentType.PlainText
                        : BodyContentType.Html
                },
                BodyPreview = message.BodyPreview,
                ReceivedDateTime = message.ReceivedDateTime,
                IsRead = message.IsRead ?? false,
                HasAttachments = message.HasAttachments ?? false,
                Attachments = message.Attachments?
                    .OfType<FileAttachment>()
                    .Select(attachment => new FlexiAttachment
                    {
                        Name = attachment.Name,
                        Bytes = attachment.ContentBytes
                    }).ToList(),
                WebLink = message.WebLink
            };
        }

        private static List<string> MapAddresses(IEnumerable<Recipient> recipients) =>
            recipients?.Select(recipient => recipient.EmailAddress?.Address)
                .Where(address => !string.IsNullOrWhiteSpace(address))
                .ToList();

        private static FlexiMailSubscription MapSubscription(Subscription subscription) =>
            subscription is null ? null : new FlexiMailSubscription
            {
                Id = subscription.Id,
                Resource = subscription.Resource,
                ChangeType = subscription.ChangeType,
                NotificationUrl = subscription.NotificationUrl,
                LifecycleNotificationUrl = subscription.LifecycleNotificationUrl,
                ExpirationDateTime = subscription.ExpirationDateTime
            };

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
