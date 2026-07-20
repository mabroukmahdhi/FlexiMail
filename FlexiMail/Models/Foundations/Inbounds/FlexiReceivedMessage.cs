// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System;
using System.Collections.Generic;
using FlexiMail.Models.Foundations.Attachments;
using FlexiMail.Models.Foundations.Bodies;

namespace FlexiMail.Models.Foundations.Inbounds
{
    /// <summary>
    /// Represents an email message received from a Microsoft Graph mailbox.
    /// </summary>
    public class FlexiReceivedMessage
    {
        /// <summary>Gets or sets the Microsoft Graph message identifier.</summary>
        public string Id { get; set; }
        /// <summary>Gets or sets the RFC message identifier assigned by the mail system.</summary>
        public string InternetMessageId { get; set; }
        /// <summary>Gets or sets the identifier of the conversation containing the message.</summary>
        public string ConversationId { get; set; }
        /// <summary>Gets or sets the sender's email address.</summary>
        public string From { get; set; }
        /// <summary>Gets or sets the primary recipient email addresses.</summary>
        public List<string> To { get; set; }
        /// <summary>Gets or sets the carbon-copy recipient email addresses.</summary>
        public List<string> Cc { get; set; }
        /// <summary>Gets or sets the message subject.</summary>
        public string Subject { get; set; }
        /// <summary>Gets or sets the message body.</summary>
        public FlexiBody Body { get; set; }
        /// <summary>Gets or sets a short preview of the message body.</summary>
        public string BodyPreview { get; set; }
        /// <summary>Gets or sets the date and time at which the message was received.</summary>
        public DateTimeOffset? ReceivedDateTime { get; set; }
        /// <summary>Gets or sets a value indicating whether the message has been read.</summary>
        public bool IsRead { get; set; }
        /// <summary>Gets or sets a value indicating whether the message has attachments.</summary>
        public bool HasAttachments { get; set; }
        /// <summary>Gets or sets the file attachments loaded with the message.</summary>
        public List<FlexiAttachment> Attachments { get; set; }
        /// <summary>Gets or sets the URL used to open the message in Outlook on the web.</summary>
        public string WebLink { get; set; }
    }
}
