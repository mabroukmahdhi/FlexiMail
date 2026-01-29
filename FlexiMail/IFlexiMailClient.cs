// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Threading.Tasks;
using FlexiMail.Models.Foundations.Messages;

namespace FlexiMail
{
    /// <summary>
    /// Defines a client capable of sending email messages and saving a copy of each sent message asynchronously.
    /// </summary>
    public interface IFlexiMailClient
    {
        /// <summary>
        /// Sends the specified message and saves a copy to the Sent Items folder asynchronously.
        /// </summary>
        /// <param name="flexiMessage">The message to send and save. Cannot be null.</param>
        /// <returns>A ValueTask that represents the asynchronous send and save operation.</returns>
        ValueTask SendAndSaveCopyAsync(FlexiMessage flexiMessage);
    }
}