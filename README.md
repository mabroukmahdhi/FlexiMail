<p align="center">
  <img src="https://github.com/mabroukmahdhi/FlexiMail/blob/main/FlexiMail/icmail.png" alt="FlexiMail logo">
</p>

# FlexiMail

**FlexiMail** is a test-driven email client for .NET 8 and C# 12 that now supports both Exchange (EWS) and Microsoft Graph through the new `FlexiGraphService`.

[![Nuget](https://img.shields.io/nuget/v/FlexiMail)](https://www.nuget.org/packages/FlexiMail/)
[![Nuget](https://img.shields.io/nuget/dt/FlexiMail)](https://www.nuget.org/packages/FlexiMail/)
![.NET 8](https://img.shields.io/badge/.NET_8-COMPATIBLE-2ea44f)

## Features

- Exchange and Microsoft Graph mail sending with sent-items copy
- `FlexiGraphService` for Graph-based delivery
- Asynchronous APIs
- Test-first design with unit and integration coverage

## Installation

```bash
dotnet add package FlexiMail
# or
Install-Package FlexiMail
```

## Usage

### Send via Exchange (EWS)
```csharp
using FlexiMail;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Bodies;
using FlexiMail.Models.Foundations.Messages;

var configurations = new ExchangeConfigurations
{
    ClientId = "your-client-id",
    ClientSecret = "your-client-secret",
    TenantId = "your-tenant-id",
    Authority = "https://login.microsoftonline.com/{tenantId}",
    Scopes = ["https://outlook.office365.com/.default"],
    SmtpAddress = "sender@domain.com"
};

var client = new FlexiMailClient(configurations);

await client.SendAndSaveCopyAsync(new FlexiMessage
{
    To = ["email@domain.com"],
    Subject = "Hello from FlexiMail",
    Body = new FlexiBody
    {
        Content = "This is the message body.",
        ContentType = BodyContentType.PlainText
    }
});
```

### Send via Microsoft Graph
```csharp
using FlexiMail;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Bodies;
using FlexiMail.Models.Foundations.Messages;

var configurations = new GraphMailConfigurations
{
    ClientId = "your-client-id",
    ClientSecret = "your-client-secret",
    TenantId = "your-tenant-id",
    SenderUserIdOrUpn = "sender@domain.com",
    Scopes = ["https://graph.microsoft.com/.default"]
};

var client = new FlexiMailClient(configurations);

await client.SendAndSaveCopyAsync(new FlexiMessage
{
    To = ["email@domain.com"],
    Subject = "Hello from FlexiGraphService",
    Body = new FlexiBody
    {
        Content = "Graph-powered delivery.",
        ContentType = BodyContentType.Html
    }
});
```

## Configuration

Example `appsettings.json` snippet:

```json
{
  "ExchangeConfigurations": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "TenantId": "your-tenant-id",
    "SmtpAddress": "sender@domain.com",
    "Authority": "https://login.microsoftonline.com/{tenantId}",
    "Scopes": ["https://outlook.office365.com/.default"]
  },
  "GraphMailConfigurations": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "TenantId": "your-tenant-id",
    "SenderUserIdOrUpn": "sender@domain.com",
    "Scopes": ["https://graph.microsoft.com/.default"]
  }
}
```

## Architecture

- **Brokers**: integrations with Exchange and Graph
- **Services**: core workflows, including `FlexiGraphService` for Graph
- **Models**: message, body, and configuration contracts

`FlexiMailClient` chooses the appropriate service based on the provided configuration and always saves a copy to Sent Items.

## Contributing

1. Fork the repository
2. Create a branch (`git checkout -b users/your-github-id/feature-name`)
3. Commit (`git commit -m "Add feature"`)
4. Push (`git push origin users/your-github-id/feature-name`)
5. Open a Pull Request

## License

MIT. See [LICENSE](https://github.com/mabroukmahdhi/FlexiMail/blob/main/LICENSE).

## Contact

For questions: [contact@mahdhi.com](mailto:contact@mahdhi.com)
