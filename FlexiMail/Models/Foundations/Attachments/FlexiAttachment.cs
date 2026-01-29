// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

namespace FlexiMail.Models.Foundations.Attachments
{

    /// <summary>
    /// Represents a file or binary attachment with a name and content.
    /// </summary>
    public class FlexiAttachment
    {
        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the raw byte data associated with this instance.
        /// </summary>
        public byte[] Bytes { get; set; }
    }
}