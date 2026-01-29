// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Threading.Tasks;
using FlexiMail.Models.Foundations.Messages;

namespace FlexiMail.Services.Graphs
{
    internal interface IFlexiGraphService
    {
        ValueTask SendAndSaveCopyAsync(FlexiMessage flexiMessage);
    }
}
