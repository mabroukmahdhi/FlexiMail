// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

namespace FlexiMail.Models.Configurations
{
    /// <summary>
    /// Defines Exchange Online app-only settings used for mailbox provisioning.
    /// </summary>
    public class ExchangeProvisioningConfigurations
    {
        /// <summary>Gets or sets the Microsoft Entra application (client) ID.</summary>
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets the Exchange Online organization, such as
        /// <c>contoso.onmicrosoft.com</c>.
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the thumbprint of the app authentication certificate in
        /// the current user's certificate store.
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets the PowerShell executable used to load the Exchange
        /// Online module. The default is <c>pwsh</c>.
        /// </summary>
        public string PowerShellExecutable { get; set; } = "pwsh";
    }
}
