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

#### Designing the notification endpoint

`NotificationUrl` receives resource-change notifications. For the subscription
created above, each notification means that a message was created in the
mailbox Inbox. The endpoint should only validate and durably enqueue the event;
email retrieval and business processing should run in a background worker.

Both webhook URLs must implement Microsoft's validation handshake. Graph sends
a `POST` with a `validationToken` query parameter, and the endpoint must return
the URL-decoded token with `200 OK`, `text/plain`, and no JSON wrapper within 10
seconds.

Minimal API notification endpoint:

```csharp
using FlexiMail.Models.Foundations.Subscriptions;
using System.Text.Json;

app.MapPost("/webhooks/fleximail", async (
    HttpRequest request,
    IMailNotificationQueue notificationQueue,
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

        // Persist to a durable queue. Do not retrieve or process the email here.
        await notificationQueue.EnqueueAsync(notification, cancellationToken);
    }

    return Results.Accepted();
});
```

`IMailNotificationQueue` represents an application-owned durable queue, such as
Azure Service Bus, RabbitMQ, Amazon SQS, or a database-backed job queue. A
background worker dequeues the event and retrieves the message:

```csharp
if (notification.ChangeType == "created" &&
    !string.IsNullOrWhiteSpace(notification.ResourceData?.Id))
{
    var message = await client.GetReceivedMessageAsync(
        notification.ResourceData.Id,
        mailbox: "support@domain.com",
        cancellationToken);

    // Process the email idempotently here.
}
```

The notification endpoint should:

1. Accept only HTTPS requests.
2. Complete the validation-token handshake before attempting JSON parsing.
3. Deserialize every item in the `value` array because Graph batches events.
4. Compare `clientState` with the secret stored for that subscription and
   discard mismatches.
5. Durably enqueue valid events and return `202 Accepted` within three seconds.
6. Return `5xx` if the event could not be persisted, allowing Graph to retry.
7. Process events idempotently because duplicate or retried notifications are
   possible. A useful deduplication key combines `SubscriptionId`, `ChangeType`,
   and `ResourceData.Id`.

Do not trust the mailbox, subscription, or message ID solely because it appears
in the request. Match `SubscriptionId` to a stored subscription and use the
stored mailbox when retrieving the message. Keep `clientState` secret and do
not log it.

#### Designing the lifecycle notification endpoint

`LifecycleNotificationUrl` receives events about subscription health, not new
emails. Its payload uses the same top-level `{ "value": [...] }` envelope and
contains a `lifecycleEvent` value. A minimal application DTO is:

```csharp
using System.Text.Json.Serialization;

public sealed class GraphLifecycleNotificationCollection
{
    [JsonPropertyName("value")]
    public List<GraphLifecycleNotification> Value { get; set; } = [];
}

public sealed class GraphLifecycleNotification
{
    [JsonPropertyName("subscriptionId")]
    public string SubscriptionId { get; set; }

    [JsonPropertyName("subscriptionExpirationDateTime")]
    public DateTimeOffset? SubscriptionExpirationDateTime { get; set; }

    [JsonPropertyName("clientState")]
    public string ClientState { get; set; }

    [JsonPropertyName("lifecycleEvent")]
    public string LifecycleEvent { get; set; }
}
```

The lifecycle endpoint follows the same handshake, validation, batching, and
queue-first rules:

```csharp
app.MapPost("/webhooks/fleximail/lifecycle", async (
    HttpRequest request,
    IMailLifecycleQueue lifecycleQueue,
    CancellationToken cancellationToken) =>
{
    if (request.Query.TryGetValue("validationToken", out var token))
    {
        return Results.Text(token.ToString(), "text/plain");
    }

    var batch = await JsonSerializer.DeserializeAsync<GraphLifecycleNotificationCollection>(
        request.Body,
        cancellationToken: cancellationToken);

    foreach (var notification in batch?.Value ?? [])
    {
        if (!string.Equals(notification.ClientState, clientState,
            StringComparison.Ordinal))
        {
            continue;
        }

        await lifecycleQueue.EnqueueAsync(notification, cancellationToken);
    }

    return Results.Accepted();
});
```

The lifecycle worker handles each event as follows:

- `reauthorizationRequired`: call `RenewSubscriptionAsync(subscriptionId)`.
  Renewal also reauthorizes the subscription. Do not concurrently send separate
  renew and reauthorize requests for the same subscription.
- `subscriptionRemoved`: create a replacement with
  `SubscribeToInboxAsync(...)`, persist its new ID and expiration, and reconcile
  the Inbox for changes that occurred during the gap.
- `missed`: reconcile the Inbox against durable local state. Microsoft Graph
  delta queries are the preferred large-scale recovery mechanism; FlexiMail
  does not currently expose delta-query APIs, so consumers must either call
  Graph directly or rescan a suitable recent Inbox window and deduplicate by
  message ID.

Subscriptions created by FlexiMail expire after six days. Store the subscription
ID, mailbox, `clientState`, and expiration in durable storage, and run a scheduled
renewal before expiration even when no lifecycle event was received. Lifecycle
events complement scheduled renewal; they do not replace it.

For both endpoints, Graph considers a notification delivered after a timely 2xx
response. Returning quickly avoids endpoint throttling and dropped events;
durable queues ensure work survives after `202 Accepted` is returned.

See Microsoft's documentation for the complete
[webhook delivery contract](https://learn.microsoft.com/en-us/graph/change-notifications-delivery-webhooks)
and [lifecycle-event behavior](https://learn.microsoft.com/en-us/graph/change-notifications-lifecycle-events).

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
