//---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
//---------------------------------------

namespace FlexiMail.Models.Configurations
{
    public class SmtpConfigurations
    {
        public string Host { get; set; }
        public int Port { get; set; } = 587;
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; } = true;
        public int Timeout { get; set; } = 10000;
        public bool UseDefaultCredentials { get; set; } = false;
    }
}