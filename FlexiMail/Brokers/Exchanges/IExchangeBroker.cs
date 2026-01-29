// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;

namespace FlexiMail.Brokers.Exchanges
{
    internal interface IExchangeBroker
    {
        ValueTask<string> GetAccessTokenAsync();

        ExchangeService CreateExchangeService(ExchangeVersion version, string accessToken,
            ImpersonatedUserId impersonatedUserId);

        void Reply(EmailMessage emailMessage, MessageBody bodyPrefix, bool replyAll);
        void Forward(EmailMessage emailMessage, MessageBody bodyPrefix, params EmailAddress[] toRecipients);
        void Forward(EmailMessage emailMessage, MessageBody bodyPrefix, IEnumerable<EmailAddress> toRecipients);
        void Send(EmailMessage emailMessage);
        void SendAndSaveCopy(EmailMessage emailMessage, FolderId destinationFolderId);
        void SendAndSaveCopy(EmailMessage emailMessage);
        void SuppressReadReceipt(EmailMessage emailMessage);
    }
}