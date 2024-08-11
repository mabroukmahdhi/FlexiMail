// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using FlexiMail.Models.Foundations.Messages;
using FlexiMail.Models.Foundations.Messages.Exceptions;

namespace FlexiMail.Services
{
    internal partial class FlexiExchangeService
    {
        private static void ValidFlexiMessage(FlexiMessage flexiMessage)
        {
            ValidFlexiMessageIsNotNull(flexiMessage);
            
            
        }

        private static void ValidFlexiMessageIsNotNull(FlexiMessage flexiMessage)
        {
            if (flexiMessage == null)
                throw new NullFlexiMessageException(message: "FlexiMessage is null.");
        }
    }
}