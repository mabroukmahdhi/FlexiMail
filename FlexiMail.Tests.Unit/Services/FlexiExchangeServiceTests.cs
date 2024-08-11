// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using FlexiMail.Brokers.Exchanges;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Attachments;
using FlexiMail.Models.Foundations.Messages;
using FlexiMail.Services;
using Microsoft.Exchange.WebServices.Data;
using Moq;
using Tynamix.ObjectFiller;

namespace FlexiMail.Tests.Unit.Services
{
    public partial class FlexiExchangeServiceTests
    {
        private readonly Mock<IExchangeBroker> exchangeBrokerMock;
        private readonly IFlexiExchangeService flexiExchangeService;

        public FlexiExchangeServiceTests()
        {
            this.exchangeBrokerMock = new Mock<IExchangeBroker>();

            var configurations = GetRandomConfigurations();

            this.flexiExchangeService = new FlexiExchangeService(
                configurations: configurations,
                exchangeBroker: this.exchangeBrokerMock.Object);
        }

        private static ExchangeConfigurations GetRandomConfigurations()
        {
            return new ExchangeConfigurations
            {
                Authority = GetRandomString(),
                Scopes = new[] { GetRandomString() },
                PrincipalName = GetRandomString(),
                Sid = GetRandomString(),
                ClientId = GetRandomString(),
                ClientSecret = GetRandomString(),
                SmtpAddress = GetRandomString(),
                TenantId = GetRandomString()
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static FlexiMessage CreateRandomFlexiMessage()
        {
            var filler = new Filler<FlexiMessage>();
            filler.Setup().OnProperty(x => x.Attachments).IgnoreIt();
            return filler.Create();
        }

        private static EmailMessage CreateEmailMessage(FlexiMessage flexiMessage, ExchangeService exchangeService)
        {
            var message = new EmailMessage(exchangeService)
            {
                Subject = flexiMessage.Subject
            };

            return message;
        }

        private static ExchangeService CreateExchangeService()
        {
            return new ExchangeService();
        }
    }
}