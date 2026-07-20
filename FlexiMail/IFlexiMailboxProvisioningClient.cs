// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FlexiMail.Models.Foundations.Mailboxes;

namespace FlexiMail
{
    /// <summary>Defines administrative Exchange Online mailbox operations.</summary>
    public interface IFlexiMailboxProvisioningClient
    {
        /// <summary>Creates an unlicensed shared mailbox in Exchange Online.</summary>
        /// <param name="request">The shared mailbox properties.</param>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>The provisioned shared mailbox.</returns>
        ValueTask<FlexiProvisionedMailbox> CreateSharedMailboxAsync(
            FlexiSharedMailboxRequest request,
            CancellationToken cancellationToken = default);
    }
}
