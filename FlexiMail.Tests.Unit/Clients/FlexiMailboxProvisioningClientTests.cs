// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System;
using System.Threading;
using FlexiMail.Brokers.Provisioning;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Mailboxes;
using Moq;

namespace FlexiMail.Tests.Unit.Clients
{
    public class FlexiMailboxProvisioningClientTests
    {
        private readonly Mock<IExchangeOnlineProvisioningBroker> brokerMock = new();

        [Fact]
        public async void ShouldCreateSharedMailboxAsync()
        {
            var configurations = CreateConfigurations();
            var request = CreateRequest();
            var client = new FlexiMailboxProvisioningClient(configurations, brokerMock.Object);

            await client.CreateSharedMailboxAsync(request, CancellationToken.None);

            brokerMock.Verify(broker => broker.CreateSharedMailboxAsync(
                request, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async void ShouldRejectInvalidAppIdAsync()
        {
            var configurations = CreateConfigurations();
            configurations.AppId = "not-a-guid";
            var client = new FlexiMailboxProvisioningClient(configurations, brokerMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                client.CreateSharedMailboxAsync(CreateRequest()).AsTask());

            brokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData("")]
        [InlineData("alias with spaces")]
        [InlineData("alias;Remove-Mailbox")]
        public async void ShouldRejectInvalidAliasAsync(string alias)
        {
            var request = CreateRequest();
            request.Alias = alias;
            var client = new FlexiMailboxProvisioningClient(CreateConfigurations(), brokerMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                client.CreateSharedMailboxAsync(request).AsTask());

            brokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldRejectInvalidSmtpAddressAsync()
        {
            var request = CreateRequest();
            request.PrimarySmtpAddress = "not-an-email-address";
            var client = new FlexiMailboxProvisioningClient(CreateConfigurations(), brokerMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                client.CreateSharedMailboxAsync(request).AsTask());

            brokerMock.VerifyNoOtherCalls();
        }

        private static ExchangeProvisioningConfigurations CreateConfigurations() => new()
        {
            AppId = Guid.NewGuid().ToString(),
            Organization = "contoso.onmicrosoft.com",
            CertificateThumbprint = "0123456789ABCDEF",
            PowerShellExecutable = "pwsh"
        };

        private static FlexiSharedMailboxRequest CreateRequest() => new()
        {
            DisplayName = "Customer Support",
            Alias = "support",
            PrimarySmtpAddress = "support@contoso.com"
        };
    }
}
