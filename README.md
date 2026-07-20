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
- Microsoft Graph inbox reading, including bodies and file attachments
- Microsoft Graph webhook subscription management for new inbox messages
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

> **Note**: The Exchange constructor of `FlexiMailClient` is compiled only for `net8.0` and `net9.0`. When targeting `net10.0`, use the Graph constructor (`FlexiMailClient(GraphMailConfigurations)`).

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

### Read received email with Microsoft Graph

Inbound APIs require a Graph-configured client. When `mailbox` is omitted,
`SenderUserIdOrUpn` is used.

```csharp
var page = await client.GetInboxAsync(
    mailbox: "support@domain.com",
    pageSize: 50,
    unreadOnly: true);

foreach (var summary in page.Messages)
{
    var message = await client.GetReceivedMessageAsync(
        messageId: summary.Id,
        mailbox: "support@domain.com");

    Console.WriteLine($"{message.ReceivedDateTime}: {message.From} - {message.Subject}");
    Console.WriteLine(message.Body?.Content);
}
```

`GetInboxAsync` requests newest messages first when reading all messages. Graph
does not guarantee ordering when the unread-only filter is used. `pageSize` must
be between 1 and 1000. `GetReceivedMessageAsync` also expands file attachments
and returns their content in `FlexiAttachment.Bytes`.

### Receive notifications for new email

The application must expose a publicly accessible HTTPS webhook. FlexiMail
creates and manages the Microsoft Graph subscription; the consuming ASP.NET
Core application hosts the endpoint.

```csharp
const string clientState = "store-this-as-a-secret";

var subscription = await client.SubscribeToInboxAsync(
    notificationUrl: "https://api.domain.com/webhooks/fleximail",
    clientState: clientState,
    mailbox: "support@domain.com",
    lifecycleNotificationUrl: "https://api.domain.com/webhooks/fleximail/lifecycle");

// Persist subscription.Id and subscription.ExpirationDateTime.
// Renew it before expiration:
subscription = await client.RenewSubscriptionAsync(subscription.Id);

// Remove it when no longer required:
await client.DeleteSubscriptionAsync(subscription.Id);
```

Minimal API webhook example:

```csharp
using FlexiMail.Models.Subscriptions;
using System.Text.Json;

app.MapPost("/webhooks/fleximail", async (
    HttpRequest request,
    CancellationToken cancellationToken) =>
{
    // Microsoft Graph validates the URL while the subscription is created.
    if (request.Query.TryGetValue("validationToken", out var token))
    {
        return Results.Text(token.ToString(), "text/plain");
    }

    var notifications = await JsonSerializer.DeserializeAsync<FlexiMailNotificationCollection>(
        request.Body,
        cancellationToken: cancellationToken);

    foreach (var notification in notifications?.Value ?? [])
    {
        if (!notification.HasClientState(clientState))
        {
            continue;
        }

        // Queue this work in production and return promptly.
        var message = await client.GetReceivedMessageAsync(
            notification.ResourceData.Id,
            mailbox: "support@domain.com",
            cancellationToken);

        // Process message here.
    }

    return Results.Accepted();
});
```

Graph validates a webhook by POSTing a `validationToken` query parameter. The
endpoint must return its URL-decoded value as `text/plain` within 10 seconds.
For normal notifications, validate `clientState`, enqueue processing, and return
`202 Accepted` quickly. Subscriptions created by FlexiMail last six days and
must be renewed. Lifecycle notifications and durable subscription storage are
recommended for production.

The Entra application needs the Microsoft Graph application permission
`Mail.Read` with administrator consent. Because this permission can read tenant
mailboxes, administrators should restrict the application's mailbox access in
Exchange Online where appropriate. `Mail.Send` remains required for sending.

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

The `Scopes` value remains `https://graph.microsoft.com/.default`; actual
permissions (`Mail.Send`, `Mail.Read`) are configured and consented on the Entra
application registration.

## Architecture

- **Brokers**: integrations with Exchange and Graph
- **Services**: core workflows, including `FlexiGraphService` for Graph
- **Models**: message, body, and configuration contracts

`FlexiMailClient` chooses the appropriate service based on the provided
configuration and always saves a copy to Sent Items. Inbox reading and
subscription management are Graph-only; calling them on an Exchange-configured
client throws `NotSupportedException`.

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
