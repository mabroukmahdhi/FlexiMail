// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Threading.Tasks;

namespace FlexiMail.Brokers.Graphs
{
    /// <summary>
    /// Defines a contract for sending email messages using Microsoft Graph on behalf of a specified user.
    /// </summary>
    public interface IGraphMailBroker
    {
        /// <summary>
        /// Sends an email message asynchronously from the specified user to the specified recipient with the given
        /// subject and HTML body.
        /// </summary>
        /// <param name="fromUserIdOrUpn">The unique identifier or user principal name (UPN) of the sender. Cannot be null or empty.</param>
        /// <param name="to">The email address of the recipient. Cannot be null or empty.</param>
        /// <param name="subject">The subject line of the email message. Cannot be null.</param>
        /// <param name="htmlBody">The HTML content to include in the body of the email message. Cannot be null.</param>
        /// <param name="saveToSentItems">true to save the sent message in the sender's Sent Items folder; otherwise, false. The default is true.</param>
        /// <returns>A ValueTask that represents the asynchronous send operation.</returns>
        ValueTask SendAsync(
            string fromUserIdOrUpn,
            string to,
            string subject,
            string htmlBody,
            bool saveToSentItems = true);
    }
}
