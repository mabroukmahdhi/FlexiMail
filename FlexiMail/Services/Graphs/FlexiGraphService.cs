// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Threading.Tasks;
using FlexiMail.Brokers.Graphs;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Messages;

namespace FlexiMail.Services.Graphs
{
    internal partial class FlexiGraphService(
        GraphMailConfigurations configurations,
        IGraphMailBroker graphMailBroker)
        : IFlexiGraphService
    {
        private readonly GraphMailConfigurations configurations = configurations;
        private readonly IGraphMailBroker graphMailBroker = graphMailBroker;

        public ValueTask SendAndSaveCopyAsync(FlexiMessage flexiMessage) =>
        TryCatch(async () =>
        {
            ValidFlexiMessage(flexiMessage);
            ValidateConfigurations();

            await this.graphMailBroker.SendAsync(
                fromUserIdOrUpn: this.configurations.SenderUserIdOrUpn,
                to: GetPrimaryRecipient(flexiMessage),
                subject: flexiMessage.Subject,
                htmlBody: GetBodyContent(flexiMessage),
                saveToSentItems: true);
        });

        private static string GetPrimaryRecipient(FlexiMessage flexiMessage)
        {
            if ((flexiMessage.To?.Count ?? 0) > 0)
            {
                return flexiMessage.To[0];
            }

            if ((flexiMessage.Cc?.Count ?? 0) > 0)
            {
                return flexiMessage.Cc[0];
            }

            return flexiMessage.Bcc[0];
        }

        private static string GetBodyContent(FlexiMessage flexiMessage) =>
            flexiMessage.Body?.Content ?? string.Empty;
    }
}
