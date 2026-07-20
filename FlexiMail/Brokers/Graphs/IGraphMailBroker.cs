// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FlexiMail.Models.Foundations.Attachments;
using FlexiMail.Models.Foundations.Bodies;
using FlexiMail.Models.Foundations.Inbounds;
using FlexiMail.Models.Foundations.Subscriptions;

namespace FlexiMail.Brokers.Graphs
{
    /// <summary>
    /// Defines a contract for sending email messages using Microsoft Graph on behalf of a specified user.
    /// </summary>
    public interface IGraphMailBroker
    {
        /// <summary>
        /// Sends an email message asynchronously from the specified user with the given recipients, subject, body and attachments.
        /// </summary>
        /// <param name="fromUserIdOrUpn">The unique identifier or user principal name (UPN) of the sender. Cannot be null or empty.</param>
        /// <param name="toRecipients">The collection of primary recipients. Can be null.</param>
        /// <param name="ccRecipients">The collection of carbon copy recipients. Can be null.</param>
        /// <param name="bccRecipients">The collection of blind carbon copy recipients. Can be null.</param>
        /// <param name="subject">The subject line of the email message. Cannot be null.</param>
        /// <param name="body">The content to include in the body of the email message. Cannot be null.</param>
        /// <param name="bodyContentType">The body content type of the email.</param>
        /// <param name="attachments">The collection of attachments to include. Can be null.</param>
        /// <param name="saveToSentItems">true to save the sent message in the sender's Sent Items folder; otherwise, false. The default is true.</param>
        /// <returns>A ValueTask that represents the asynchronous send operation.</returns>
        ValueTask SendAsync(
            string fromUserIdOrUpn,
            IEnumerable<string> toRecipients,
            IEnumerable<string> ccRecipients,
            IEnumerable<string> bccRecipients,
            string subject,
            string body,
            BodyContentType bodyContentType,
            IEnumerable<FlexiAttachment> attachments,
            bool saveToSentItems = true);

        /// <summary>Retrieves a page of messages from the specified mailbox Inbox.</summary>
        /// <param name="mailbox">The mailbox user ID or UPN.</param>
        /// <param name="pageSize">The maximum number of messages to return.</param>
        /// <param name="unreadOnly">Whether to return only unread messages.</param>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>The requested page of messages.</returns>
        ValueTask<FlexiReceivedMessagePage> GetInboxAsync(string mailbox, int pageSize, bool unreadOnly, CancellationToken cancellationToken);

        /// <summary>Retrieves a message from the specified mailbox.</summary>
        /// <param name="mailbox">The mailbox user ID or UPN.</param>
        /// <param name="messageId">The Microsoft Graph message identifier.</param>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>The received message.</returns>
        ValueTask<FlexiReceivedMessage> GetMessageAsync(string mailbox, string messageId, CancellationToken cancellationToken);

        /// <summary>Creates a new-message subscription for the specified mailbox Inbox.</summary>
        /// <param name="mailbox">The mailbox user ID or UPN.</param>
        /// <param name="notificationUrl">The HTTPS notification endpoint.</param>
        /// <param name="lifecycleNotificationUrl">The optional HTTPS lifecycle endpoint.</param>
        /// <param name="clientState">The secret notification validation value.</param>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>The created subscription.</returns>
        ValueTask<FlexiMailSubscription> CreateInboxSubscriptionAsync(string mailbox, string notificationUrl, string lifecycleNotificationUrl, string clientState, CancellationToken cancellationToken);

        /// <summary>Renews an existing mail subscription.</summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>The renewed subscription.</returns>
        ValueTask<FlexiMailSubscription> RenewSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken);

        /// <summary>Deletes an existing mail subscription.</summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        ValueTask DeleteSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken);
    }
}
