// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Messages;
using FlexiMail.Models.Foundations.Messages.Exceptions;

namespace FlexiMail.Services.Graphs
{
    internal partial class FlexiGraphService
    {
        private void ValidFlexiMessage(FlexiMessage flexiMessage)
        {
            ValidFlexiMessageIsNotNull(flexiMessage);
            ValidateConfigurations();

            if ((flexiMessage.To?.Count ?? 0) > 0
                || (flexiMessage.Cc?.Count ?? 0) > 0
                || (flexiMessage.Bcc?.Count ?? 0) > 0)
            {
                return;
            }

            Validate((Rule: IsInvalid(flexiMessage.To), Parameter: nameof(FlexiMessage.To)),
                (Rule: IsInvalid(flexiMessage.Cc), Parameter: nameof(FlexiMessage.Cc)),
                (Rule: IsInvalid(flexiMessage.Bcc), Parameter: nameof(FlexiMessage.Bcc)));
        }

        private void ValidateConfigurations() =>
            Validate((Rule: IsInvalidSender(this.configurations.SenderUserIdOrUpn),
                Parameter: nameof(GraphMailConfigurations.SenderUserIdOrUpn)));

        private static void ValidFlexiMessageIsNotNull(FlexiMessage flexiMessage)
        {
            if (flexiMessage == null)
            {
                throw new NullFlexiMessageException(message: "FlexiMessage is null.");
            }
        }

        private static dynamic IsInvalid(object @object) => new
        {
            Condition = @object is null,
            Message = "Value is not set"
        };

        private static dynamic IsInvalidSender(string sender) => new
        {
            Condition = string.IsNullOrWhiteSpace(sender),
            Message = "Value is not set"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidCompletionException =
                new InvalidFlexiMessageException(
                    message: "Invalid message error occurred, fix errors and try again.");

            foreach (var (rule, parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidCompletionException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidCompletionException.ThrowIfContainsErrors();
        }
    }
}
