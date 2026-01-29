// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Collections.Generic;
using FlexiMail.Models.Foundations.Attachments;
using FlexiMail.Models.Foundations.Bodies;

namespace FlexiMail.Models.Foundations.Messages
{
    /// <summary>
    /// Represents an email message with support for multiple recipients, subject, body content, and attachments.
    /// </summary>
    /// <remarks>The FlexiMessage class provides properties for specifying primary, carbon copy (Cc), and
    /// blind carbon copy (Bcc) recipients, as well as the message subject, body, and any file attachments. Use this
    /// class to construct an email message before sending it with a compatible email service or client. The Body
    /// property must be set to a valid FlexiBody instance to define the message content. The Attachments property can
    /// be used to include one or more files with the message.</remarks>
    public class FlexiMessage
    {
        /// <summary>
        /// Gets or sets the list of recipient email addresses.
        /// </summary>
        public List<string> To { get; set; }

        /// <summary>
        /// Gets or sets the list of email addresses to receive a carbon copy (Cc) of the message.
        /// </summary>
        public List<string> Cc { get; set; }

        /// <summary>
        /// Gets or sets the list of email addresses to receive blind carbon copies (Bcc) of the message.
        /// </summary>
        public List<string> Bcc { get; set; }

        /// <summary>
        /// Gets or sets the subject line of the message.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body content for the current instance.
        /// </summary>
        public FlexiBody Body { get; set; }

        /// <summary>
        /// Gets or sets the collection of attachments associated with this instance.
        /// </summary>
        public List<FlexiAttachment> Attachments { get; set; }
    }
}