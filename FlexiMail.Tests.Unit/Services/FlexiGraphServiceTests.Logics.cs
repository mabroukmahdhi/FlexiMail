// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Linq;
using FlexiMail.Services.Graphs;
using Moq;

namespace FlexiMail.Tests.Unit.Services
{
    public partial class FlexiGraphServiceTests
    {
        [Fact]
        public async void ShouldSendAndSaveCopyAsync()
        {
            // given
            var randomMessage = CreateRandomFlexiMessage();

            var configurations = GetRandomConfigurations();
            var sender = configurations.SenderUserIdOrUpn;

            var flexiGraphService = new FlexiGraphService(
                configurations: configurations,
                graphMailBroker: this.graphMailBrokerMock.Object);

            var expectedRecipient = randomMessage.To.First();
            var expectedSubject = randomMessage.Subject;
            var expectedBody = randomMessage.Body.Content;

            // when
            await flexiGraphService.SendAndSaveCopyAsync(randomMessage);

            // then
            this.graphMailBrokerMock.Verify(broker =>
                    broker.SendAsync(
                        sender,
                        expectedRecipient,
                        expectedSubject,
                        expectedBody,
                        true),
                Times.Once);
        }

        [Theory]
        [InlineData(true, false, false)] // Only To address is added
        [InlineData(false, true, false)] // Only Cc address is added
        [InlineData(false, false, true)] // Only Bcc address is added
        public async void ShouldSendAndSaveCopyBasedOnAddressTypeAsync(bool hasTo, bool hasCc, bool hasBcc)
        {
            // given
            var randomMessage = CreateRandomFlexiMessage();

            if (!hasTo)
            {
                randomMessage.To = null;
            }

            if (!hasCc)
            {
                randomMessage.Cc = null;
            }

            if (!hasBcc)
            {
                randomMessage.Bcc = null;
            }

            var configurations = GetRandomConfigurations();
            var sender = configurations.SenderUserIdOrUpn;

            var flexiGraphService = new FlexiGraphService(
                configurations: configurations,
                graphMailBroker: this.graphMailBrokerMock.Object);

            var expectedRecipient = hasTo
                ? randomMessage.To.First()
                    : hasCc ? randomMessage.Cc.First() : randomMessage.Bcc.First();

            // when
            await flexiGraphService.SendAndSaveCopyAsync(randomMessage);

            // then
            this.graphMailBrokerMock.Verify(broker =>
                    broker.SendAsync(
                        sender,
                        expectedRecipient,
                        randomMessage.Subject,
                        randomMessage.Body.Content,
                        true),
                Times.Once);
        }
    }
}
