// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Collections.Generic;
using FlexiMail.Models.Foundations.Attachments;
using FlexiMail.Models.Foundations.Bodies;

namespace FlexiMail.Models.Foundations.Messages
{
    public class FlexiMessage
    {
        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }
        public string Subject { get; set; }
        public FlexiBody Body { get; set; }
        public List<FlexiAttachment> Attachments { get; set; }
    }
}