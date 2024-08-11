//---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
//---------------------------------------

using System;
using FlexiMail.Brokers.Exchanges;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Messages;
using FlexiMail.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FlexiMail
{
    public class FlexiMailClient : IFlexiMailClient
    {
        private readonly IFlexiExchangeService exchangeService;

        public FlexiMailClient(ExchangeConfigurations configurations)
        {
            var serviceProvider = RegisterServices(configurations);

            this.exchangeService =
                serviceProvider.GetRequiredService<IFlexiExchangeService>();
        }

        public void SendAndSaveCopyAsync(FlexiMessage flexiMessage) =>
            this.exchangeService.SendAndSaveCopyAsync(flexiMessage);

        private static IServiceProvider RegisterServices(ExchangeConfigurations configurations)
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient<IExchangeBroker, ExchangeBroker>()
                .AddTransient<IFlexiExchangeService, FlexiExchangeService>()
                .AddSingleton(configurations);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}