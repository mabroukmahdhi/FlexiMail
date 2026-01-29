// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FlexiMail.Models.Foundations.Attachments;
using FlexiMail.Models.Foundations.Bodies;

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
    }
}
