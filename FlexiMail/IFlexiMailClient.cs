// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FlexiMail.Models.Foundations.Inbounds;
using FlexiMail.Models.Foundations.Messages;
using FlexiMail.Models.Foundations.Subscriptions;

namespace FlexiMail
{
    /// <summary>
    /// Defines a client capable of sending email messages and saving a copy of each sent message asynchronously.
    /// </summary>
    public interface IFlexiMailClient
    {
        /// <summary>
        /// Sends the specified message and saves a copy to the Sent Items folder asynchronously.
        /// </summary>
        /// <param name="flexiMessage">The message to send and save. Cannot be null.</param>
        /// <returns>A ValueTask that represents the asynchronous send and save operation.</returns>
        ValueTask SendAndSaveCopyAsync(FlexiMessage flexiMessage);

        /// <summary>
        /// Retrieves a page of messages from a Microsoft Graph mailbox Inbox.
        /// </summary>
        /// <param name="mailbox">The mailbox user ID or UPN. Defaults to the configured sender.</param>
        /// <param name="pageSize">The number of messages to request, from 1 through 1000.</param>
        /// <param name="unreadOnly">Whether to return only unread messages.</param>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>The requested page of received messages.</returns>
        ValueTask<FlexiReceivedMessagePage> GetInboxAsync(
            string mailbox = null,
            int pageSize = 25,
            bool unreadOnly = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves one received message, including its file attachments, from Microsoft Graph.
        /// </summary>
        /// <param name="messageId">The Microsoft Graph message identifier.</param>
        /// <param name="mailbox">The mailbox user ID or UPN. Defaults to the configured sender.</param>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>The received message, or <see langword="null"/> if Graph returns no message.</returns>
        ValueTask<FlexiReceivedMessage> GetReceivedMessageAsync(
            string messageId,
            string mailbox = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a Microsoft Graph change-notification subscription for newly created Inbox messages.
        /// </summary>
        /// <param name="notificationUrl">The publicly accessible HTTPS notification endpoint.</param>
        /// <param name="clientState">A secret value returned with notifications for origin validation.</param>
        /// <param name="mailbox">The mailbox user ID or UPN. Defaults to the configured sender.</param>
        /// <param name="lifecycleNotificationUrl">An optional publicly accessible HTTPS lifecycle endpoint.</param>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>The created subscription.</returns>
        ValueTask<FlexiMailSubscription> SubscribeToInboxAsync(
            string notificationUrl,
            string clientState,
            string mailbox = null,
            string lifecycleNotificationUrl = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Renews a Microsoft Graph mail subscription for six days.
        /// </summary>
        /// <param name="subscriptionId">The Microsoft Graph subscription identifier.</param>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>The renewed subscription.</returns>
        ValueTask<FlexiMailSubscription> RenewSubscriptionAsync(
            string subscriptionId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a Microsoft Graph mail subscription.
        /// </summary>
        /// <param name="subscriptionId">The Microsoft Graph subscription identifier.</param>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        ValueTask DeleteSubscriptionAsync(
            string subscriptionId,
            CancellationToken cancellationToken = default);
    }
}
