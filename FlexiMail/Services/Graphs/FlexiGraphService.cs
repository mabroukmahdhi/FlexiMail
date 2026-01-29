// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Threading.Tasks;
using FlexiMail.Brokers.Graphs;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Bodies;
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
                toRecipients: flexiMessage.To,
                ccRecipients: flexiMessage.Cc,
                bccRecipients: flexiMessage.Bcc,
                subject: flexiMessage.Subject,
                body: GetBodyContent(flexiMessage),
                bodyContentType: GetBodyContentType(flexiMessage),
                attachments: flexiMessage.Attachments,
                saveToSentItems: true);
        });

        private static string GetBodyContent(FlexiMessage flexiMessage) =>
            flexiMessage.Body?.Content ?? string.Empty;

        private static BodyContentType GetBodyContentType(FlexiMessage flexiMessage) =>
            flexiMessage.Body?.ContentType ?? BodyContentType.Html;
    }
}
