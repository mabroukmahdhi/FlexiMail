// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using Xeptions;

namespace FlexiMail.Models.Foundations.Messages.Exceptions
{
    /// <summary>
    /// Represents errors that occur when a Flexi message is invalid or cannot be processed.
    /// </summary>
    /// <remarks>This exception is typically thrown when a Flexi message does not conform to the expected
    /// format or contains invalid data. Use this exception to distinguish Flexi message validation errors from other
    /// types of exceptions.</remarks>
    public class InvalidFlexiMessageException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the InvalidFlexiMessageException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidFlexiMessageException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the InvalidFlexiMessageException class with a specified error message and a
        /// reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is
        /// specified.</param>
        public InvalidFlexiMessageException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}