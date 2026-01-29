// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using Xeptions;

namespace FlexiMail.Models.Foundations.Messages.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a Flexi message is found to be null.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public class NullFlexiMessageException(string message) : Xeption(message)
    {
    }
}