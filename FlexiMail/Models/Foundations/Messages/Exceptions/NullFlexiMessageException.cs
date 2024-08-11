// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using Xeptions;

namespace FlexiMail.Models.Foundations.Messages.Exceptions
{
    public class NullFlexiMessageException(string message) : Xeption(message)
    {
    }
}