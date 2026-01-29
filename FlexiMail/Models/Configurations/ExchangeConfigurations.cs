//---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
//---------------------------------------

namespace FlexiMail.Models.Configurations
{
    /// <summary>
    /// Represents the configuration settings required to connect to an Exchange service using application credentials.
    /// </summary>
    /// <remarks>This class encapsulates authentication and connection details such as client credentials,
    /// tenant information, and required scopes. It is typically used to provide configuration data for services or
    /// components that interact with Microsoft Exchange via OAuth or similar authentication mechanisms.</remarks>
    public class ExchangeConfigurations
    {
        /// <summary>
        /// Gets or sets the unique identifier for the client application.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret used for authenticating the application with an external service.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the tenant associated with the current context.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the SMTP email address associated with the account.
        /// </summary>
        public string SmtpAddress { get; set; }

        /// <summary>
        /// Gets or sets the security identifier (SID) associated with the entity.
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Gets or sets the user principal name (UPN) associated with the account.
        /// </summary>
        public string PrincipalName { get; set; }

        /// <summary>
        /// Gets or sets the authority component of the Uniform Resource Identifier (URI).
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Gets or sets the set of OAuth scopes to request during authentication.
        /// </summary>
        /// <remarks>Each scope defines a specific permission that the application is requesting from the
        /// authentication provider. The required scopes depend on the APIs or resources the application needs to
        /// access.</remarks>
        public string[] Scopes { get; set; }
    }
}