//---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
//---------------------------------------

namespace FlexiMail.Models.Configurations
{
    public class ExchangeConfigurations
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
        public string SmtpAddress { get; set; }
        public string Sid { get; set; }
        public string PrincipalName { get; set; }
        public string Authority { get; set; }
        public string[] Scopes { get; set; }
    }
}