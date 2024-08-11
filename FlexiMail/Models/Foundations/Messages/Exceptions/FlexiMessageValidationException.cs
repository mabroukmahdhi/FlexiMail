// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System;

namespace FlexiMail.Models.Foundations.Messages.Exceptions
{
    public class FlexiMessageValidationException(string message, Exception innerException)
        : Exception(message, innerException)
    {
    }
}