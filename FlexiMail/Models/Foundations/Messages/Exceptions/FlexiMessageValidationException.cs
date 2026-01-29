// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using Xeptions;

namespace FlexiMail.Models.Foundations.Messages.Exceptions
{
    /// <summary>
    /// Represents errors that occur when a FlexiMessage fails validation.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the validation failure.</param>
    /// <param name="innerException">The exception that is the cause of this exception, or a null reference if no inner exception is specified.</param>
    public class FlexiMessageValidationException(string message, Xeption innerException)
        : Xeption(message, innerException)
    {
    }
}