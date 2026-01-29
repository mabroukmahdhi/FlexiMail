// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using FlexiMail.Brokers.Graphs;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Bodies;
using FlexiMail.Models.Foundations.Messages;
using FlexiMail.Services.Graphs;
using Moq;
using Tynamix.ObjectFiller;

namespace FlexiMail.Tests.Unit.Services
{
    public partial class FlexiGraphServiceTests
    {
        private readonly Mock<IGraphMailBroker> graphMailBrokerMock;
        private readonly IFlexiGraphService flexiGraphService;

        public FlexiGraphServiceTests()
        {
            this.graphMailBrokerMock = new Mock<IGraphMailBroker>();

            var configurations = GetRandomConfigurations();

            this.flexiGraphService = new FlexiGraphService(
                configurations: configurations,
                graphMailBroker: this.graphMailBrokerMock.Object);
        }

        private static GraphMailConfigurations GetRandomConfigurations()
        {
            return new GraphMailConfigurations
            {
                TenantId = GetRandomString(),
                ClientId = GetRandomString(),
                ClientSecret = GetRandomString(),
                Scopes = [GetRandomString()],
                SenderUserIdOrUpn = GetRandomString()
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static FlexiMessage CreateRandomFlexiMessage() =>
            new FlexiMessage
            {
                To = [GetRandomString()],
                Cc = [GetRandomString()],
                Bcc = [GetRandomString()],
                Subject = GetRandomString(),
                Body = new FlexiBody
                {
                    Content = GetRandomString(),
                    ContentType = BodyContentType.Html
                }
            };
    }
}
