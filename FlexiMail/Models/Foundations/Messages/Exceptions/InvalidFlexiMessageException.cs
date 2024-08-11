// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using Xeptions;

namespace FlexiMail.Models.Foundations.Messages.Exceptions
{
    public class InvalidFlexiMessageException : Xeption
    {
        public InvalidFlexiMessageException(string message)
            : base(message)
        { }

        public InvalidFlexiMessageException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}