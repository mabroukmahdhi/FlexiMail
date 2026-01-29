// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

namespace FlexiMail.Models.Foundations.Bodies
{
    /// <summary>
    /// Represents a flexible body of content with an associated content type.
    /// </summary>
    public class FlexiBody
    {
        /// <summary>
        /// Gets or sets the textual content associated with this instance.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the type of content represented by the body.
        /// </summary>
        public BodyContentType ContentType { get; set; }
    }
}