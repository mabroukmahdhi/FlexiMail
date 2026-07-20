// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

namespace FlexiMail.Models.Foundations.Mailboxes
{
    /// <summary>Describes a shared mailbox to create in Exchange Online.</summary>
    public class FlexiSharedMailboxRequest
    {
        /// <summary>Gets or sets the mailbox display name.</summary>
        public string DisplayName { get; set; }

        /// <summary>Gets or sets the unique Exchange alias.</summary>
        public string Alias { get; set; }

        /// <summary>Gets or sets the primary SMTP address.</summary>
        public string PrimarySmtpAddress { get; set; }
    }
}
