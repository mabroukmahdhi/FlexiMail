// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

namespace FlexiMail.Models.Foundations.Mailboxes
{
    /// <summary>Represents a mailbox provisioned in Exchange Online.</summary>
    public class FlexiProvisionedMailbox
    {
        /// <summary>Gets or sets the Exchange identity.</summary>
        public string Identity { get; set; }

        /// <summary>Gets or sets the Microsoft Entra object ID.</summary>
        public string ExternalDirectoryObjectId { get; set; }

        /// <summary>Gets or sets the mailbox display name.</summary>
        public string DisplayName { get; set; }

        /// <summary>Gets or sets the Exchange alias.</summary>
        public string Alias { get; set; }

        /// <summary>Gets or sets the primary SMTP address.</summary>
        public string PrimarySmtpAddress { get; set; }

        /// <summary>Gets or sets the detailed Exchange recipient type.</summary>
        public string RecipientTypeDetails { get; set; }
    }
}
