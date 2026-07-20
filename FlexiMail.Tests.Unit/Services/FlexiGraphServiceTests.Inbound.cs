// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Threading;
using FlexiMail.Models.Configurations;
using FlexiMail.Services.Graphs;
using Moq;

namespace FlexiMail.Tests.Unit.Services
{
    public partial class FlexiGraphServiceTests
    {
        [Fact]
        public async void ShouldReadConfiguredMailboxInboxAsync()
        {
            var configurations = GetRandomConfigurations();
            var service = new FlexiGraphService(configurations, this.graphMailBrokerMock.Object);

            await service.GetInboxAsync(null, 25, true, CancellationToken.None);

            this.graphMailBrokerMock.Verify(broker => broker.GetInboxAsync(
                configurations.SenderUserIdOrUpn, 25, true, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async void ShouldReadExplicitMailboxMessageAsync()
        {
            var mailbox = GetRandomString();
            var messageId = GetRandomString();
            var service = new FlexiGraphService(GetRandomConfigurations(), this.graphMailBrokerMock.Object);

            await service.GetReceivedMessageAsync(messageId, mailbox, CancellationToken.None);

            this.graphMailBrokerMock.Verify(broker => broker.GetMessageAsync(
                mailbox, messageId, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async void ShouldCreateInboxSubscriptionAsync()
        {
            var configurations = GetRandomConfigurations();
            var clientState = GetRandomString();
            const string notificationUrl = "https://example.com/webhooks/mail";
            var service = new FlexiGraphService(configurations, this.graphMailBrokerMock.Object);

            await service.SubscribeToInboxAsync(
                notificationUrl, clientState, null, null, CancellationToken.None);

            this.graphMailBrokerMock.Verify(broker => broker.CreateInboxSubscriptionAsync(
                configurations.SenderUserIdOrUpn, notificationUrl, null,
                clientState, CancellationToken.None), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1001)]
        public async void ShouldRejectInvalidInboxPageSizeAsync(int pageSize)
        {
            await Assert.ThrowsAsync<System.ArgumentOutOfRangeException>(() =>
                this.flexiGraphService.GetInboxAsync(null, pageSize, false, CancellationToken.None).AsTask());
        }
    }
}
