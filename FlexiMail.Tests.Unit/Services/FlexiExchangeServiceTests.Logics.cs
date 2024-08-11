// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using Microsoft.Exchange.WebServices.Data;
using Moq;

namespace FlexiMail.Tests.Unit.Services
{
    public partial class FlexiExchangeServiceTests
    {
        [Fact]
        public async void ShouldSendAndSaveCopyAsync()
        {
            // given
            var randomAccessToken = GetRandomString();
            var randomMessage = CreateRandomFlexiMessage();
            var randomExchangeService = CreateExchangeService();

            this.exchangeBrokerMock.Setup(broker =>
                    broker.GetAccessTokenAsync())
                .ReturnsAsync(randomAccessToken);

            this.exchangeBrokerMock.Setup(broker =>
                    broker.CreateExchangeService(
                        ExchangeVersion.Exchange2013,
                        randomAccessToken,
                        It.IsAny<ImpersonatedUserId>()))
                .Returns(randomExchangeService);

            // when
            await this.flexiExchangeService.SendAndSaveCopyAsync(randomMessage);

            // then
            this.exchangeBrokerMock.Verify(broker =>
                    broker.GetAccessTokenAsync(),
                Times.Once);

            this.exchangeBrokerMock.Verify(broker =>
                    broker.CreateExchangeService(
                        ExchangeVersion.Exchange2013,
                        randomAccessToken,
                        It.IsAny<ImpersonatedUserId>()),
                Times.Once);

            this.exchangeBrokerMock.Verify(broker =>
                    broker.SendAndSaveCopy(It.IsAny<EmailMessage>()),
                Times.Once);
        }

        [Theory]
        [InlineData(true, false, false)] // Only To address is added
        [InlineData(false, true, false)] // Only Cc address is added
        [InlineData(false, false, true)] // Only Bcc address is added
        public async void ShouldSendAndSaveCopyBasedOnAddressTypeAsync(bool hasTo, bool hasCc, bool hasBcc)
        {
            // given
            var randomAccessToken = GetRandomString();
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

            var randomExchangeService = CreateExchangeService();

            this.exchangeBrokerMock.Setup(broker =>
                    broker.GetAccessTokenAsync())
                .ReturnsAsync(randomAccessToken);

            this.exchangeBrokerMock.Setup(broker =>
                    broker.CreateExchangeService(
                        ExchangeVersion.Exchange2013,
                        randomAccessToken,
                        It.IsAny<ImpersonatedUserId>()))
                .Returns(randomExchangeService);

            // when
            await this.flexiExchangeService.SendAndSaveCopyAsync(randomMessage);

            // then
            this.exchangeBrokerMock.Verify(broker =>
                    broker.GetAccessTokenAsync(),
                Times.Once);

            this.exchangeBrokerMock.Verify(broker =>
                    broker.CreateExchangeService(
                        ExchangeVersion.Exchange2013,
                        randomAccessToken,
                        It.IsAny<ImpersonatedUserId>()),
                Times.Once);

            this.exchangeBrokerMock.Verify(broker =>
                    broker.SendAndSaveCopy(It.IsAny<EmailMessage>()),
                Times.Once);
        }
    }
}