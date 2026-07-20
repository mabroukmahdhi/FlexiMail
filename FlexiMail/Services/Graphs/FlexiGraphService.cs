// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FlexiMail.Brokers.Graphs;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Bodies;
using FlexiMail.Models.Foundations.Inbounds;
using FlexiMail.Models.Foundations.Messages;
using FlexiMail.Models.Foundations.Subscriptions;

namespace FlexiMail.Services.Graphs
{
    internal partial class FlexiGraphService(
        GraphMailConfigurations configurations,
        IGraphMailBroker graphMailBroker)
        : IFlexiGraphService
    {
        private readonly GraphMailConfigurations configurations = configurations;
        private readonly IGraphMailBroker graphMailBroker = graphMailBroker;

        public ValueTask SendAndSaveCopyAsync(FlexiMessage flexiMessage) =>
        TryCatch(async () =>
        {
            ValidFlexiMessage(flexiMessage);
            ValidateConfigurations();

            await this.graphMailBroker.SendAsync(
                fromUserIdOrUpn: this.configurations.SenderUserIdOrUpn,
                toRecipients: flexiMessage.To,
                ccRecipients: flexiMessage.Cc,
                bccRecipients: flexiMessage.Bcc,
                subject: flexiMessage.Subject,
                body: GetBodyContent(flexiMessage),
                bodyContentType: GetBodyContentType(flexiMessage),
                attachments: flexiMessage.Attachments,
                saveToSentItems: true);
        });

        private static string GetBodyContent(FlexiMessage flexiMessage) =>
            flexiMessage.Body?.Content ?? string.Empty;

        private static BodyContentType GetBodyContentType(FlexiMessage flexiMessage) =>
            flexiMessage.Body?.ContentType ?? BodyContentType.Html;

        public ValueTask<FlexiReceivedMessagePage> GetInboxAsync(
            string mailbox, int pageSize, bool unreadOnly, CancellationToken cancellationToken)
        {
            var resolvedMailbox = ResolveMailbox(mailbox);

            if (pageSize < 1 || pageSize > 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be between 1 and 1000.");
            }

            return this.graphMailBroker.GetInboxAsync(
                resolvedMailbox, pageSize, unreadOnly, cancellationToken);
        }

        public ValueTask<FlexiReceivedMessage> GetReceivedMessageAsync(
            string messageId, string mailbox, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(messageId);
            return this.graphMailBroker.GetMessageAsync(ResolveMailbox(mailbox), messageId, cancellationToken);
        }

        public ValueTask<FlexiMailSubscription> SubscribeToInboxAsync(
            string notificationUrl,
            string clientState,
            string mailbox,
            string lifecycleNotificationUrl,
            CancellationToken cancellationToken)
        {
            ValidateHttpsUrl(notificationUrl, nameof(notificationUrl));
            ArgumentException.ThrowIfNullOrWhiteSpace(clientState);

            if (!string.IsNullOrWhiteSpace(lifecycleNotificationUrl))
            {
                ValidateHttpsUrl(lifecycleNotificationUrl, nameof(lifecycleNotificationUrl));
            }

            return this.graphMailBroker.CreateInboxSubscriptionAsync(
                ResolveMailbox(mailbox), notificationUrl, lifecycleNotificationUrl,
                clientState, cancellationToken);
        }

        public ValueTask<FlexiMailSubscription> RenewSubscriptionAsync(
            string subscriptionId, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(subscriptionId);
            return this.graphMailBroker.RenewSubscriptionAsync(subscriptionId, cancellationToken);
        }

        public ValueTask DeleteSubscriptionAsync(
            string subscriptionId, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(subscriptionId);
            return this.graphMailBroker.DeleteSubscriptionAsync(subscriptionId, cancellationToken);
        }

        private string ResolveMailbox(string mailbox)
        {
            var resolvedMailbox = string.IsNullOrWhiteSpace(mailbox)
                ? this.configurations.SenderUserIdOrUpn
                : mailbox;

            return !string.IsNullOrWhiteSpace(resolvedMailbox)
                ? resolvedMailbox
                : throw new ArgumentException("A mailbox or SenderUserIdOrUpn must be provided.", nameof(mailbox));
        }

        private static void ValidateHttpsUrl(string value, string parameterName)
        {
            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
            {
                throw new ArgumentException("A publicly accessible HTTPS URL is required.", parameterName);
            }
        }
    }
}
