// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FlexiMail.Models.Foundations.Inbounds;
using FlexiMail.Models.Foundations.Messages;
using FlexiMail.Models.Foundations.Subscriptions;

namespace FlexiMail.Services.Graphs
{
    internal interface IFlexiGraphService
    {
        ValueTask SendAndSaveCopyAsync(FlexiMessage flexiMessage);
        ValueTask<FlexiReceivedMessagePage> GetInboxAsync(string mailbox, int pageSize, bool unreadOnly, CancellationToken cancellationToken);
        ValueTask<FlexiReceivedMessage> GetReceivedMessageAsync(string messageId, string mailbox, CancellationToken cancellationToken);
        ValueTask<FlexiMailSubscription> SubscribeToInboxAsync(string notificationUrl, string clientState, string mailbox, string lifecycleNotificationUrl, CancellationToken cancellationToken);
        ValueTask<FlexiMailSubscription> RenewSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken);
        ValueTask DeleteSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken);
    }
}
