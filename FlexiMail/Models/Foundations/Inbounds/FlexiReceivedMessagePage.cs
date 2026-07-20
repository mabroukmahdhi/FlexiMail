// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Collections.Generic;

namespace FlexiMail.Models.Foundations.Inbounds
{
    /// <summary>
    /// Represents one page of messages returned from a Microsoft Graph mailbox.
    /// </summary>
    public class FlexiReceivedMessagePage
    {
        /// <summary>Gets or sets the messages in the current page.</summary>
        public List<FlexiReceivedMessage> Messages { get; set; } = [];
        /// <summary>Gets or sets the Microsoft Graph URL for the next page, when one exists.</summary>
        public string NextLink { get; set; }
    }
}
