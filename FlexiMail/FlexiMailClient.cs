//---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
//---------------------------------------

using System;
using System.Threading.Tasks;
#if !NET10_0
using FlexiMail.Brokers.Exchanges;
#endif
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

#if NET8_0 || NET9_0
        /// <summary>
        /// Initializes a new instance of the FlexiMailClient class configured to use Exchange services.
        /// </summary>
        /// <remarks>This constructor sets up the client to interact with Exchange only. The Graph service
        /// is not initialized when using this constructor.</remarks>
        /// <param name="configurations">The ExchangeConfigurations object that specifies the settings required to connect to the Exchange service.
        /// Cannot be null.</param>
        public FlexiMailClient(ExchangeConfigurations configurations)
        {
            flexiClientService = FlexiClientService.Exchange;

            var serviceProvider = RegisterServices(
                exchangeConfigurations: configurations,
                graphMailConfigurations: new GraphMailConfigurations());

            this.exchangeService =
                serviceProvider.GetRequiredService<IFlexiExchangeService>();
            this.graphService = null;
        }
#endif

        /// <summary>
        /// Initializes a new instance of the FlexiMailClient class using the specified Graph mail configurations.
        /// </summary>
        /// <param name="configurations">The GraphMailConfigurations object that defines the settings for connecting to the Graph mail service.
        /// Cannot be null.</param>
        public FlexiMailClient(GraphMailConfigurations configurations)
        {
            flexiClientService = FlexiClientService.Graph;

            var serviceProvider = RegisterServices(
                exchangeConfigurations: new ExchangeConfigurations(),
                graphMailConfigurations: configurations);

            this.exchangeService = null;
            this.graphService =
                serviceProvider.GetRequiredService<IFlexiGraphService>();
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

        private static IServiceProvider RegisterServices(
            ExchangeConfigurations exchangeConfigurations,
            GraphMailConfigurations graphMailConfigurations)
        {
            var serviceCollection = new ServiceCollection();

#if !NET10_0
            serviceCollection.AddTransient<IExchangeBroker, ExchangeBroker>()
                    .AddTransient<IFlexiExchangeService, FlexiExchangeService>()
                        .AddSingleton(exchangeConfigurations);
#endif

            serviceCollection.AddTransient<IGraphMailBroker, GraphMailBroker>()
                .AddTransient<IFlexiGraphService, FlexiGraphService>()
                    .AddSingleton(graphMailConfigurations);

            return serviceCollection.BuildServiceProvider();
        }
    }
}