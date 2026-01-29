// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Messages;
using FlexiMail.Models.Foundations.Messages.Exceptions;
using FlexiMail.Services.Graphs;
using FluentAssertions;
using Moq;

namespace FlexiMail.Tests.Unit.Services
{
    public partial class FlexiGraphServiceTests
    {
        [Fact]
        public async void ShouldThrowValidationExceptionIfFlexiMessageIsNull()
        {
            // given 
            FlexiMessage nullMessage = null;

            var nullFlexiMessageException =
                new NullFlexiMessageException(
                    message: "FlexiMessage is null.");

            var expectedFlexiMessageValidationException =
                new FlexiMessageValidationException(
                    message: "Flexi Message validation error occurred, fix errors and try again.",
                    innerException: nullFlexiMessageException);

            // when
            var sendMessageTask =
                this.flexiGraphService.SendAndSaveCopyAsync(nullMessage);

            var actualFlexiMessageValidationException =
                await Assert.ThrowsAsync<FlexiMessageValidationException>(
                    sendMessageTask.AsTask);
            // then

            actualFlexiMessageValidationException.Should()
                .BeEquivalentTo(expectedFlexiMessageValidationException);

            this.graphMailBrokerMock.Verify(broker =>
                    broker.SendAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<bool>()),
                Times.Never);
        }

        [Fact]
        public async void ShouldThrowValidationExceptionIfNoReceiverWasAdded()
        {
            // given 
            var randomFlexiMessage = CreateRandomFlexiMessage();

            randomFlexiMessage.To = null;
            randomFlexiMessage.Cc = null;
            randomFlexiMessage.Bcc = null;

            var inputFlexiMessage = randomFlexiMessage;

            var invalidFlexiMessageException =
                new InvalidFlexiMessageException(
                    message: "Invalid message error occurred, fix errors and try again.");

            invalidFlexiMessageException.AddData(
                key: nameof(FlexiMessage.To),
                values: "Value is not set");

            invalidFlexiMessageException.AddData(
                key: nameof(FlexiMessage.Cc),
                values: "Value is not set");

            invalidFlexiMessageException.AddData(
                key: nameof(FlexiMessage.Bcc),
                values: "Value is not set");

            var expectedFlexiMessageValidationException =
                new FlexiMessageValidationException(
                    message: "Flexi Message validation error occurred, fix errors and try again.",
                    innerException: invalidFlexiMessageException);

            // when
            var sendMessageTask =
                this.flexiGraphService.SendAndSaveCopyAsync(inputFlexiMessage);

            var actualFlexiMessageValidationException =
                await Assert.ThrowsAsync<FlexiMessageValidationException>(
                    sendMessageTask.AsTask);
            // then

            actualFlexiMessageValidationException.Should()
                .BeEquivalentTo(expectedFlexiMessageValidationException);

            this.graphMailBrokerMock.Verify(broker =>
                    broker.SendAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<bool>()),
                Times.Never);
        }

        [Fact]
        public async void ShouldThrowValidationExceptionIfSenderIsNotSet()
        {
            // given 
            var randomFlexiMessage = CreateRandomFlexiMessage();

            var invalidFlexiMessageException =
                new InvalidFlexiMessageException(
                    message: "Invalid message error occurred, fix errors and try again.");

            invalidFlexiMessageException.AddData(
                key: nameof(GraphMailConfigurations.SenderUserIdOrUpn),
                values: "Value is not set");

            var expectedFlexiMessageValidationException =
                new FlexiMessageValidationException(
                    message: "Flexi Message validation error occurred, fix errors and try again.",
                    innerException: invalidFlexiMessageException);

            var configurations = GetRandomConfigurations();
            configurations.SenderUserIdOrUpn = null;

            var graphService = new FlexiGraphService(
                configurations: configurations,
                graphMailBroker: this.graphMailBrokerMock.Object);

            // when
            var sendMessageTask =
                graphService.SendAndSaveCopyAsync(randomFlexiMessage);

            var actualFlexiMessageValidationException =
                await Assert.ThrowsAsync<FlexiMessageValidationException>(
                    sendMessageTask.AsTask);
            // then

            actualFlexiMessageValidationException.Should()
                .BeEquivalentTo(expectedFlexiMessageValidationException);

            this.graphMailBrokerMock.Verify(broker =>
                    broker.SendAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<bool>()),
                Times.Never);
        }
    }
}
