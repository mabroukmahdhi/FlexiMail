// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FlexiMail.Brokers.Exchanges;
using FlexiMail.Extensions;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Bodies;
using FlexiMail.Models.Foundations.Messages;
using Microsoft.Exchange.WebServices.Data;

namespace FlexiMail.Services
{
    internal class FlexiExchangeService(
        ExchangeConfigurations configurations, 
        IExchangeBroker exchangeBroker)
        : IFlexiExchangeService
    {
        private readonly IExchangeBroker exchangeBroker = exchangeBroker;
        
        public async void SendAndSaveCopyAsync(FlexiMessage flexiMessage)
        {
            var exchangeService = await CreateExchangeServiceAsync();
            var emailMessage = new EmailMessage(service: exchangeService)
            {
                Subject = flexiMessage.Subject,
                Body = new MessageBody(
                    bodyType: MapBodyType(bodyContentType: flexiMessage.Body.ContentType),
                    text: flexiMessage.Body.Content)
            };

            emailMessage.ToRecipients.AddAddresses(flexiMessage.To);
            emailMessage.CcRecipients.AddAddresses(flexiMessage.Cc);
            emailMessage.BccRecipients.AddAddresses(flexiMessage.Bcc);

            var tempFiles = new List<string>();

            if (flexiMessage.Attachments != null)
            {
                foreach (var attachment in flexiMessage.Attachments)
                {
                    var tempFilePath = Path.Combine(Path.GetTempPath(), attachment.Name);
                    await File.WriteAllBytesAsync(tempFilePath, attachment.Bytes);

                    tempFiles.Add(tempFilePath);

                    emailMessage.Attachments.AddFileAttachment(attachment.Name, tempFilePath);
                }
            }

            this.exchangeBroker.SendAndSaveCopy(emailMessage: emailMessage);

            foreach (var filePath in tempFiles)
            {
                File.Delete(filePath);
            }
        }
        private async ValueTask<ExchangeService> CreateExchangeServiceAsync()
        {
            var accessToken = await this.exchangeBroker.GetAccessTokenAsync();

            var idType = GetConnectingIdType();
            var impersonatedUserId = new ImpersonatedUserId(
                idType: idType,
                id: GetUserId(idType));

            return this.exchangeBroker.CreateExchangeService(ExchangeVersion.Exchange2013, accessToken,
                impersonatedUserId);
        }
        private string GetUserId(ConnectingIdType idType) => idType switch
        {
            ConnectingIdType.PrincipalName => configurations.PrincipalName,
            ConnectingIdType.SmtpAddress => configurations.SmtpAddress,
            ConnectingIdType.SID => configurations.Sid,
            _ => configurations.SmtpAddress
        };
        private ConnectingIdType GetConnectingIdType()
        {
            if (!string.IsNullOrWhiteSpace(configurations.SmtpAddress))
                return ConnectingIdType.SmtpAddress;

            if (!string.IsNullOrWhiteSpace(configurations.Sid))
                return ConnectingIdType.SID;

            return !string.IsNullOrWhiteSpace(configurations.PrincipalName)
                ? ConnectingIdType.PrincipalName
                : ConnectingIdType.SmtpAddress;
        }
        private static BodyType MapBodyType(BodyContentType bodyContentType) => bodyContentType switch
        {
            BodyContentType.Html => BodyType.HTML,
            BodyContentType.PlainText => BodyType.Text,
            _ => BodyType.HTML
        };
    }
}