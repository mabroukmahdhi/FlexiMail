# FlexiMail manual tests

This console application exercises Microsoft Graph inbox reading and mail
subscription management against a real tenant.

Before running, replace the placeholders at the top of `Program.cs`:

- `TenantId`, `ClientId`, and `ClientSecret`
- `SenderUserIdOrUpn`
- `NotificationUrl` and `LifecycleNotificationUrl`
- `ClientState` with a long random secret
- `ProvisioningConfigurations` with the Exchange app ID, tenant organization,
  and certificate thumbprint when testing shared-mailbox creation

The Entra application requires the Microsoft Graph application permission
`Mail.Read` with administrator consent. The notification URLs must be publicly
accessible HTTPS endpoints that implement Microsoft Graph webhook validation.

Run the menu with:

```powershell
dotnet run --project FlexiMail.Tests.Manual
```

The subscription scenarios create a six-day Inbox subscription and allow its
ID to be renewed or deleted. A subscription created during the current process
is remembered automatically; an ID can also be pasted from an earlier run.

Shared-mailbox creation also requires PowerShell 7, the
`ExchangeOnlineManagement` module, the `Exchange.ManageAsApp` application
permission with admin consent, and the supported Microsoft Entra **Exchange
Administrator** directory role assigned to the enterprise application for the
initial test. The console requires typing `CREATE` before it performs this
administrative operation.
