// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using FlexiMail.Models.Foundations.Bodies;
using FlexiMail.Models.Foundations.Messages;

namespace FlexiMail.Test.Integration.Clients
{
    public partial class FlexiMailClientTests
    {
        [Fact]
        public async void ShouldSendAndCopyMessageAsync()
        {
            // Given
            var flexiMessage = new FlexiMessage()
            {
                Subject = "FlexiMail Integration Test",
                To = [GetReceiverTestEmail()],
                Body = new FlexiBody
                {
                    Content = "<h3>This is a test</h3><p>Bonjour tout le monde!</p>",
                    ContentType = BodyContentType.Html
                }
            };

            // when then
            await this.flexiMailClient.SendAndSaveCopyAsync(flexiMessage);
        }
    }
}