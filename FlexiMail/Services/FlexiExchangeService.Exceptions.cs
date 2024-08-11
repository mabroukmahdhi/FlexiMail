// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Threading.Tasks;
using FlexiMail.Models.Foundations.Messages.Exceptions;
using Xeptions;

namespace FlexiMail.Services
{
    internal partial class FlexiExchangeService
    {
        private delegate ValueTask SendFunction();

        private static async ValueTask TryCatch(SendFunction sendFunction)
        {
            try
            {
                await sendFunction();
            }
            catch (NullFlexiMessageException nullFlexiMessageException)
            {
                throw CreateValidationException(nullFlexiMessageException);
            }
            catch (InvalidFlexiMessageException invalidFlexiMessageException)
            {
                throw CreateValidationException(invalidFlexiMessageException);
            }
        }

        private static FlexiMessageValidationException CreateValidationException(Xeption exception)
        {
            return new FlexiMessageValidationException(
                message: "Flexi Message validation error occurred, fix errors and try again.",
                innerException: exception);
        }
    }
}