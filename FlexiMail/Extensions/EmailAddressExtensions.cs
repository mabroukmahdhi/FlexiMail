// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------
#if !NET10_0
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;

namespace FlexiMail.Extensions
{
    /// <summary>
    /// Provides convenience methods for adding email addresses to EWS collections.
    /// </summary>
    public static class EmailAddressExtensions
    {
        /// <summary>
        /// Adds each supplied address to an Exchange Web Services email-address collection.
        /// </summary>
        /// <param name="emailAddressCollection">The collection to which addresses are added.</param>
        /// <param name="addresses">The addresses to add. A <see langword="null"/> list is ignored.</param>
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
#endif
