using System.Threading;
using System.Threading.Tasks;
using FlexiMail.Models.Foundations.Mailboxes;

namespace FlexiMail.Brokers.Provisioning
{
    internal interface IExchangeOnlineProvisioningBroker
    {
        ValueTask<FlexiProvisionedMailbox> CreateSharedMailboxAsync(
            FlexiSharedMailboxRequest request,
            CancellationToken cancellationToken);
    }
}
