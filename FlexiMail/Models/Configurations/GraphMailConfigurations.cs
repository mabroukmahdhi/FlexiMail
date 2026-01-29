// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

namespace FlexiMail.Models.Configurations
{
    /// <summary>
    /// Represents configuration settings required to authenticate and send email using Microsoft Graph API.
    /// </summary>
    /// <remarks>This class encapsulates the credentials and scope information necessary for applications to
    /// access Microsoft Graph services for sending email. All properties must be set with valid values before use.
    /// Typically used to configure Graph API clients in applications that automate email sending.</remarks>
    public class GraphMailConfigurations
    {
        /// <summary>
        /// Gets or sets the unique identifier of the tenant associated with the current context.
        /// </summary>
        public string TenantId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier for the client application.
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the client secret used for authenticating with the external service.
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the set of OAuth scopes to request when acquiring an access token.
        /// </summary>
        /// <remarks>The default value is "https://graph.microsoft.com/.default", which requests the
        /// application's default permissions for Microsoft Graph. Modify this property to specify additional or
        /// different scopes as required by your application.</remarks> 
        public string[] Scopes { get; set; } = ["https://graph.microsoft.com/.default"];

        /// <summary>
        /// Gets or sets the unique identifier or user principal name (UPN) of the account used to send mail.
        /// </summary>
        public string SenderUserIdOrUpn { get; set; } = string.Empty;
    }
}
