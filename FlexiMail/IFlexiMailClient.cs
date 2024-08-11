// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using FlexiMail.Models.Foundations.Messages;

namespace FlexiMail
{
    public interface IFlexiMailClient
    {
        void SendAndSaveCopyAsync(FlexiMessage flexiMessage);
    }
}