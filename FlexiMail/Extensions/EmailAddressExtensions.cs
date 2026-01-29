// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;

namespace FlexiMail.Extensions
{
    public static class EmailAddressExtensions
    {
        public static void AddAddresses(this EmailAddressCollection emailAddressCollection, List<string> addresses)
        {
            if (addresses == null)
            {
                return;
            }

            foreach (var cc in addresses)
            {
                emailAddressCollection?.Add(cc);
            }
        }
    }
}