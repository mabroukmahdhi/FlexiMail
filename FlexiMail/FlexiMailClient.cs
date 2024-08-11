//---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
//---------------------------------------

using System;
using System.Threading.Tasks;
using FlexiMail.Brokers.Exchanges;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Messages;
using FlexiMail.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FlexiMail
{
    /// <summary>
    /// Client class to send mails through Exchange WebServices.
    /// </summary>
    public class FlexiMailClient : IFlexiMailClient
    {
        private readonly IFlexiExchangeService exchangeService;

        /// <summary>
        /// Creates Client Instance.
        /// </summary>
        /// <param name="configurations">The Exchange Configurations</param>
        public FlexiMailClient(ExchangeConfigurations configurations)
        {
            var serviceProvider = RegisterServices(configurations);

            this.exchangeService =
                serviceProvider.GetRequiredService<IFlexiExchangeService>();
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="flexiMessage">The email model</param>
        public async ValueTask SendAndSaveCopyAsync(FlexiMessage flexiMessage) =>
            await this.exchangeService.SendAndSaveCopyAsync(flexiMessage);

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