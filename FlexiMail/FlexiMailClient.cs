//---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
//---------------------------------------

using System;
using System.Threading.Tasks;
using FlexiMail.Brokers.Exchanges;
using FlexiMail.Brokers.Graphs;
using FlexiMail.Models.Clients;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Messages;
using FlexiMail.Services;
using FlexiMail.Services.Graphs;
using Microsoft.Extensions.DependencyInjection;

namespace FlexiMail
{
    /// <summary>
    /// Provides a client for sending email messages using either Exchange or Microsoft Graph mail services.
    /// </summary>
    /// <remarks>Use this class to send email messages and save copies to the sender's Sent Items folder. The
    /// client can be initialized with either Exchange or Graph mail configurations, allowing integration with different
    /// mail service providers. This class is not thread-safe; create a separate instance for each concurrent operation
    /// if needed.</remarks>
    public class FlexiMailClient : IFlexiMailClient
    {
        private readonly IFlexiExchangeService exchangeService;
        private readonly IFlexiGraphService graphService;
        private FlexiClientService flexiClientService;

        /// <summary>
        /// Creates Client Instance.
        /// </summary>
        /// <param name="configurations">The Exchange Configurations</param>
        [Obsolete(message: "Use FlexiMailClient(GraphMailConfigurations) instead.")]
        public FlexiMailClient(ExchangeConfigurations configurations)
        {
            flexiClientService = FlexiClientService.Exchange;
            var serviceProvider = RegisterExchangeServices(configurations);

            this.exchangeService =
                serviceProvider.GetRequiredService<IFlexiExchangeService>();
            this.graphService = null;
        }

        /// <summary>
        /// Initializes a new instance of the FlexiMailClient class using the specified Graph mail configurations.
        /// </summary>
        /// <param name="configurations">The GraphMailConfigurations object that defines the settings for connecting to the Graph mail service.
        /// Cannot be null.</param>
        public FlexiMailClient(GraphMailConfigurations configurations)
        {
            flexiClientService = FlexiClientService.Graph;
            var serviceProvider = RegisterGraphServices(new GraphMailConfigurations());

            this.exchangeService =
                serviceProvider.GetRequiredService<IFlexiExchangeService>();
        }

        /// <summary>
        /// Sends the specified message and saves a copy to the sender's Sent Items folder asynchronously.
        /// </summary>
        /// <param name="flexiMessage">The message to send and save. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous send and save operation.</returns>
        public async ValueTask SendAndSaveCopyAsync(FlexiMessage flexiMessage)
        {
            if (flexiClientService == FlexiClientService.Graph)
            {
                await this.graphService.SendAndSaveCopyAsync(flexiMessage);
                return;
            }

            await this.exchangeService.SendAndSaveCopyAsync(flexiMessage);
        }

        private static IServiceProvider RegisterExchangeServices(ExchangeConfigurations configurations)
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient<IExchangeBroker, ExchangeBroker>()
                    .AddTransient<IFlexiExchangeService, FlexiExchangeService>()
                        .AddSingleton(configurations);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }

        private static IServiceProvider RegisterGraphServices(GraphMailConfigurations configurations)
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient<IGraphMailBroker, GraphMailBroker>()
                .AddTransient<IFlexiGraphService, FlexiGraphService>()
                .AddSingleton(configurations);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}