// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

namespace FlexiMail.Models.Foundations.Bodies
{
    /// <summary>
    /// Specifies the format of the body content in a message or document.
    /// </summary>
    public enum BodyContentType
    {
        /// <summary>
        /// Gets or sets the raw HTML content to be rendered.
        /// </summary>
        /// <remarks>This property is typically used to output pre-formatted HTML markup directly to the
        /// response. Use caution to avoid introducing security vulnerabilities such as cross-site scripting (XSS) when
        /// setting this value from untrusted sources.</remarks>
        Html,

        /// <summary>
        /// Gets or sets the plain text content associated with this instance.
        /// </summary>
        PlainText
    }
}