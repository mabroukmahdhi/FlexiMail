# FlexiMail Exchange Online provisioning guide

This guide configures unattended shared-mailbox creation with
`FlexiMailboxProvisioningClient`. It covers the complete setup from a new
Microsoft Entra app registration through the FlexiMail manual test.

FlexiMail creates shared mailboxes by starting PowerShell 7 and running the
Microsoft-supported Exchange Online cmdlet `New-Mailbox -Shared`. Microsoft
Graph cannot create a shared mailbox.

## What you need

- A Microsoft 365 tenant with an Exchange Online subscription.
- An account allowed to create app registrations and grant admin consent.
- A Privileged Role Administrator or Global Administrator to assign the
  Exchange Administrator role to the application.
- Windows for the current certificate-thumbprint implementation.
- PowerShell 7 and the `ExchangeOnlineManagement` module.
- .NET 8 or later to run the manual console project.

A shared mailbox can store up to 50 GB without its own Exchange license. A
license is required for more than 50 GB, archiving, litigation hold, and certain
compliance features. A normal user mailbox cannot be provisioned without an
Exchange Online license.

## 1. Create a dedicated app registration

Use a separate provisioning application in production. Do not give mailbox
administration rights to the app used only for sending and reading email.

1. Open the [Microsoft Entra admin center](https://entra.microsoft.com/).
2. Go to **Identity > Applications > App registrations**.
3. Select **New registration**.
4. Enter a name such as `FlexiMail-Provisioning`.
5. Select **Accounts in this organizational directory only**.
6. Leave the redirect URI empty.
7. Select **Register**.
8. On **Overview**, copy these values:
   - **Application (client) ID**: used as `AppId`.
   - **Directory (tenant) ID**: useful for tenant administration, but not used
     as `AppId`.

Do not use the application Object ID as `AppId`.

## 2. Add the Exchange application permission

1. In the new app registration, open **API permissions**.
2. Select **Add a permission**.
3. Select **APIs my organization uses**.
4. Search for and select **Office 365 Exchange Online**.
5. Select **Application permissions**.
6. Expand **Exchange** and select `Exchange.ManageAsApp`.
7. Select **Add permissions**.
8. Select **Grant admin consent for your organization**.

The final entry must show:

```text
Office 365 Exchange Online
Exchange.ManageAsApp
Application
Granted for <your tenant>
```

Delegated permission is not sufficient because FlexiMail runs without an
interactive user.

## 3. Assign the supported Exchange directory role

The API permission permits app-only Exchange authentication. An Exchange role
determines which administrative operations the app may perform.

For initial setup and manual testing:

1. In Entra, go to **Identity > Roles & admins**.
2. Search for **Exchange Administrator**.
3. Open the role and select **Add assignments**.
4. Search for `FlexiMail-Provisioning`.
5. Select its enterprise application/service principal and confirm.

Search using the exact application name if applications are not listed by
default. Verify that the selected application has the expected client ID.

`Exchange Administrator` is a broad role, but it is supported by Exchange
Online app-only PowerShell and is useful for proving the integration. For
production, work with an Exchange administrator to replace it with a tested,
custom Exchange Online role group that exposes only the required recipient
creation commands and write scope.

Do not substitute any of the following:

- Azure subscription `Owner` or `Contributor`.
- A Microsoft Graph mail permission.
- An arbitrary custom Entra directory role.
- An Exchange Application RBAC role intended for Graph mailbox-data access.

## 4. Install PowerShell 7

Open Windows PowerShell or Terminal and run:

```powershell
winget install --id Microsoft.PowerShell --source winget
```

Close and reopen Visual Studio and all terminal windows so they receive the new
`PATH`. Verify the installation:

```powershell
pwsh --version
Get-Command pwsh.exe
```

The standard installation path is usually:

```text
C:\Program Files\PowerShell\7\pwsh.exe
```

## 5. Install the Exchange Online module

Start PowerShell 7:

```powershell
pwsh
```

Install the module for the current Windows account:

```powershell
Install-Module ExchangeOnlineManagement -Scope CurrentUser
```

If prompted to trust PSGallery, review the prompt and accept it. Verify the
module:

```powershell
Get-Module -ListAvailable ExchangeOnlineManagement |
    Sort-Object Version -Descending |
    Select-Object -First 1 Name, Version, Path
```

Install the module under the same Windows account that runs the FlexiMail
process. A module installed for one user might not be visible to another user,
IIS application-pool identity, scheduled task, or Windows service.

## 6. Create the authentication certificate

Open **Windows PowerShell** as the same Windows account that will run FlexiMail.
Create an RSA certificate in that user's personal certificate store:

```powershell
$certificate = New-SelfSignedCertificate `
    -Subject "CN=FlexiMail Exchange Provisioning" `
    -CertStoreLocation "Cert:\CurrentUser\My" `
    -KeyAlgorithm RSA `
    -KeyLength 2048 `
    -HashAlgorithm SHA256 `
    -KeySpec Signature `
    -KeyExportPolicy NonExportable `
    -NotAfter (Get-Date).AddYears(2)

$certificate.Thumbprint
```

Copy the returned thumbprint. It becomes `CertificateThumbprint` in the
FlexiMail configuration.

Verify the certificate:

```powershell
$certificate |
    Format-List Subject, Thumbprint, HasPrivateKey, NotBefore, NotAfter
```

`HasPrivateKey` must be `True`.

The example creates a non-exportable private key. This is appropriate when the
application will run on the same machine and user account. For deployment to a
different machine or service identity, use your organization's certificate
issuance and secure key-deployment process instead of copying an unprotected
private key.

## 7. Export and upload the public certificate

Export only the public certificate:

```powershell
Export-Certificate `
    -Cert $certificate `
    -FilePath "$env:USERPROFILE\Desktop\FlexiMail-Provisioning.cer"
```

Upload it to the app registration:

1. Return to **App registrations > FlexiMail-Provisioning**.
2. Open **Certificates & secrets > Certificates**.
3. Select **Upload certificate**.
4. Upload `FlexiMail-Provisioning.cer`.
5. Confirm that the thumbprint shown by Entra matches:

   ```powershell
   $certificate.Thumbprint
   ```

Upload the certificate under **App registrations**, not under enterprise
application SAML signing certificates. Never upload a `.pfx` containing the
private key to Entra.

## 8. Find the Exchange organization value

Use the tenant's primary `onmicrosoft.com` domain, for example:

```text
contoso.onmicrosoft.com
```

You can find it in Entra under **Identity > Overview > Primary domain**, or
under **Custom domain names**. Use this domain as `Organization`, even when the
shared mailbox will use a custom address such as `support@contoso.com`.

## 9. Test Exchange authentication directly

Close any old Exchange sessions so the test acquires a fresh token after role
or permission changes. In PowerShell 7, run:

```powershell
Connect-ExchangeOnline `
    -AppId "YOUR_APPLICATION_CLIENT_ID" `
    -CertificateThumbprint "YOUR_CERTIFICATE_THUMBPRINT" `
    -Organization "yourtenant.onmicrosoft.com"

Get-Command New-Mailbox

Disconnect-ExchangeOnline -Confirm:$false
```

Do not continue until `Connect-ExchangeOnline` succeeds and
`Get-Command New-Mailbox` returns the command. Permission and role changes can take several
minutes to propagate. Always start a new session after waiting.

## 10. Configure the FlexiMail manual test

Open `FlexiMail.Tests.Manual/Program.cs` and set:

```csharp
private static readonly ExchangeProvisioningConfigurations
    ProvisioningConfigurations = new()
{
    AppId = "YOUR_APPLICATION_CLIENT_ID",
    Organization = "yourtenant.onmicrosoft.com",
    CertificateThumbprint = "YOUR_CERTIFICATE_THUMBPRINT",
    PowerShellExecutable = @"C:\Program Files\PowerShell\7\pwsh.exe"
};
```

Using the complete executable path avoids failures when Visual Studio has not
refreshed its `PATH` after PowerShell installation.

Build and run:

```powershell
dotnet build FlexiMail.Tests.Manual\FlexiMail.Tests.Manual.csproj
dotnet run --project FlexiMail.Tests.Manual
```

Select:

```text
7 - Create an Exchange Online shared mailbox
```

Enter the display name, alias, and primary SMTP address. The console requires
typing `CREATE` before it performs the external administrative operation.

## 11. Use the provisioning client in an application

```csharp
using FlexiMail;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Mailboxes;

var client = new FlexiMailboxProvisioningClient(
    new ExchangeProvisioningConfigurations
    {
        AppId = "YOUR_APPLICATION_CLIENT_ID",
        Organization = "yourtenant.onmicrosoft.com",
        CertificateThumbprint = "YOUR_CERTIFICATE_THUMBPRINT",
        PowerShellExecutable = @"C:\Program Files\PowerShell\7\pwsh.exe"
    });

var mailbox = await client.CreateSharedMailboxAsync(
    new FlexiSharedMailboxRequest
    {
        DisplayName = "Customer Support",
        Alias = "support",
        PrimarySmtpAddress = "support@contoso.com"
    });

Console.WriteLine(mailbox.PrimarySmtpAddress);
```

Treat mailbox creation as an administrative operation. Log an audit record,
require authorization in the calling application, and avoid exposing this API
directly to untrusted clients.

## Troubleshooting

### Could not start `pwsh`

PowerShell 7 is not installed or is not visible in the process `PATH`.

```powershell
Test-Path "C:\Program Files\PowerShell\7\pwsh.exe"
```

Install PowerShell 7, restart Visual Studio, or configure the full path in
`PowerShellExecutable`.

### `ExchangeOnlineManagement` cannot be loaded

Install and verify the module from PowerShell 7 under the same account that
runs FlexiMail:

```powershell
Install-Module ExchangeOnlineManagement -Scope CurrentUser
Get-Module -ListAvailable ExchangeOnlineManagement
```

### `AADSTS700027: certificate ... is not registered on application`

The local certificate does not match a public certificate uploaded to the app
identified by `AppId`.

```powershell
Get-Item "Cert:\CurrentUser\My\YOUR_THUMBPRINT" |
    Format-List Thumbprint, HasPrivateKey, NotAfter
```

Export that exact certificate to `.cer` and upload it to the exact app
registration. Recreating a certificate produces a new key and thumbprint, even
when the subject name is unchanged.

### `The role assigned to application ... isn't supported`

Certificate authentication succeeded, but the service principal has an
unsupported directory role. Assign the Microsoft Entra **Exchange
Administrator** role to the enterprise application for the initial test.
Remove unrelated directory-role assignments added specifically for this setup.
Do not remove the `Exchange.ManageAsApp` API permission.

### Access denied or `New-Mailbox` is unavailable

Check all of the following:

- `Exchange.ManageAsApp` is an **Application** permission.
- Admin consent shows **Granted**.
- The Exchange Administrator role is assigned to the enterprise application,
  not to a similarly named user or app-registration object.
- The certificate is valid and has a private key.
- `Organization` is the tenant's primary `onmicrosoft.com` domain.
- A fresh PowerShell process was started after permission changes propagated.

### The error contains `#&lt; CLIXML`

PowerShell serializes error records as CLIXML when standard error is redirected.
The useful Exchange or Entra error text is inside the payload. Look for tokens
such as `AADSTS`, `AccessDenied`, or `role assigned`. The CLIXML wrapper itself
is not the root cause.

## Production checklist

- Use a dedicated provisioning app registration.
- Replace broad Exchange Administrator access with tested least privilege.
- Restrict who can invoke mailbox provisioning.
- Store audit records for every request and result.
- Monitor certificate expiration and rotate before `NotAfter`.
- Upload the replacement public certificate before switching the application
  to its new private key.
- Never commit certificates, private keys, passwords, tenant secrets, or real
  thumbprints to source control.
- Run the process under a dedicated service identity with access to the private
  certificate.

## Microsoft documentation

- [App-only authentication for Exchange Online PowerShell](https://learn.microsoft.com/en-us/powershell/exchange/app-only-auth-powershell-v2)
- [Connect-ExchangeOnline](https://learn.microsoft.com/en-us/powershell/module/exchangepowershell/connect-exchangeonline)
- [New-Mailbox](https://learn.microsoft.com/en-us/powershell/module/exchangepowershell/new-mailbox)
- [Create a self-signed certificate for Entra authentication](https://learn.microsoft.com/en-us/entra/identity-platform/howto-create-self-signed-certificate)
- [Shared-mailbox licensing](https://learn.microsoft.com/en-us/microsoft-365/admin/email/about-shared-mailboxes)
