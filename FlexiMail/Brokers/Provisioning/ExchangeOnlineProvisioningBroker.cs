// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Mailboxes;

namespace FlexiMail.Brokers.Provisioning
{
    internal sealed class ExchangeOnlineProvisioningBroker(
        ExchangeProvisioningConfigurations configurations)
        : IExchangeOnlineProvisioningBroker
    {
        public async ValueTask<FlexiProvisionedMailbox> CreateSharedMailboxAsync(
            FlexiSharedMailboxRequest request,
            CancellationToken cancellationToken)
        {
            var encodedCommand = Convert.ToBase64String(
                Encoding.Unicode.GetBytes(CreateScript(request)));

            var startInfo = new ProcessStartInfo
            {
                FileName = configurations.PowerShellExecutable,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add("-NoLogo");
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-NonInteractive");
            startInfo.ArgumentList.Add("-EncodedCommand");
            startInfo.ArgumentList.Add(encodedCommand);

            using var process = new Process { StartInfo = startInfo };

            try
            {
                process.Start();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    $"Could not start '{configurations.PowerShellExecutable}'. " +
                    "Install PowerShell 7 and the ExchangeOnlineManagement module.",
                    exception);
            }

            var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
            string output;
            string error;

            try
            {
                await process.WaitForExitAsync(cancellationToken);
                output = await outputTask;
                error = await errorTask;
            }
            catch (OperationCanceledException)
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }

                throw;
            }

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    "Exchange Online shared-mailbox creation failed. " +
                    GetUsefulError(error, output));
            }

            var json = output.Split(
                    ['\r', '\n'],
                    StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault(line => line.TrimStart().StartsWith("{", StringComparison.Ordinal));

            if (json is null)
            {
                throw new InvalidOperationException(
                    "Exchange Online returned no mailbox result. " +
                    GetUsefulError(error, output));
            }

            return JsonSerializer.Deserialize<FlexiProvisionedMailbox>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        private string CreateScript(FlexiSharedMailboxRequest request) => $@"
$ErrorActionPreference = 'Stop'
Import-Module ExchangeOnlineManagement -ErrorAction Stop
Connect-ExchangeOnline -AppId '{Escape(configurations.AppId)}' -CertificateThumbprint '{Escape(configurations.CertificateThumbprint)}' -Organization '{Escape(configurations.Organization)}' -ShowBanner:$false -CommandName New-Mailbox -ErrorAction Stop
try {{
    $mailbox = New-Mailbox -Shared -Name '{Escape(request.DisplayName)}' -DisplayName '{Escape(request.DisplayName)}' -Alias '{Escape(request.Alias)}' -PrimarySmtpAddress '{Escape(request.PrimarySmtpAddress)}' -ErrorAction Stop
    [PSCustomObject]@{{
        Identity = [string]$mailbox.Identity
        ExternalDirectoryObjectId = [string]$mailbox.ExternalDirectoryObjectId
        DisplayName = [string]$mailbox.DisplayName
        Alias = [string]$mailbox.Alias
        PrimarySmtpAddress = [string]$mailbox.PrimarySmtpAddress
        RecipientTypeDetails = [string]$mailbox.RecipientTypeDetails
    }} | ConvertTo-Json -Compress
}}
finally {{
    Disconnect-ExchangeOnline -Confirm:$false -ErrorAction SilentlyContinue
}}";

        private static string Escape(string value) =>
            value?.Replace("'", "''", StringComparison.Ordinal);

        private static string GetUsefulError(string error, string output)
        {
            var message = !string.IsNullOrWhiteSpace(error) ? error : output;
            return string.IsNullOrWhiteSpace(message)
                ? "The PowerShell process exited without an error message."
                : message.Trim();
        }
    }
}
