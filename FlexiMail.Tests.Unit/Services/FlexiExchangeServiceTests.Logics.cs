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
    }
}