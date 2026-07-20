// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FlexiMail.Brokers.Provisioning;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Mailboxes;

namespace FlexiMail
{
    /// <summary>
    /// Provides administrative shared-mailbox provisioning through Exchange
    /// Online PowerShell app-only authentication.
    /// </summary>
    public sealed class FlexiMailboxProvisioningClient : IFlexiMailboxProvisioningClient
    {
        private static readonly Regex AliasPattern = new(
            @"^[A-Za-z0-9.!#$%&'*+\-/=?^_`{|}~]+$",
            RegexOptions.CultureInvariant);

        private static readonly Regex ThumbprintPattern = new(
            "^[A-Fa-f0-9]+$",
            RegexOptions.CultureInvariant);

        private readonly ExchangeProvisioningConfigurations configurations;
        private readonly IExchangeOnlineProvisioningBroker broker;

        /// <summary>Initializes a new Exchange Online provisioning client.</summary>
        /// <param name="configurations">The app-only Exchange Online settings.</param>
        public FlexiMailboxProvisioningClient(
            ExchangeProvisioningConfigurations configurations)
            : this(configurations, new ExchangeOnlineProvisioningBroker(configurations))
        {
        }

        internal FlexiMailboxProvisioningClient(
            ExchangeProvisioningConfigurations configurations,
            IExchangeOnlineProvisioningBroker broker)
        {
            this.configurations = configurations;
            this.broker = broker;
        }

        /// <inheritdoc/>
        public ValueTask<FlexiProvisionedMailbox> CreateSharedMailboxAsync(
            FlexiSharedMailboxRequest request,
            CancellationToken cancellationToken = default)
        {
            ValidateConfigurations();
            ValidateRequest(request);
            return this.broker.CreateSharedMailboxAsync(request, cancellationToken);
        }

        private void ValidateConfigurations()
        {
            ArgumentNullException.ThrowIfNull(this.configurations);
            ArgumentException.ThrowIfNullOrWhiteSpace(this.configurations.AppId);
            ArgumentException.ThrowIfNullOrWhiteSpace(this.configurations.Organization);
            ArgumentException.ThrowIfNullOrWhiteSpace(this.configurations.CertificateThumbprint);
            ArgumentException.ThrowIfNullOrWhiteSpace(this.configurations.PowerShellExecutable);

            if (!Guid.TryParse(this.configurations.AppId, out _))
            {
                throw new ArgumentException("AppId must be a valid GUID.", nameof(configurations.AppId));
            }

            if (!ThumbprintPattern.IsMatch(this.configurations.CertificateThumbprint))
            {
                throw new ArgumentException(
                    "CertificateThumbprint must contain hexadecimal characters only.",
                    nameof(configurations.CertificateThumbprint));
            }
        }

        private static void ValidateRequest(FlexiSharedMailboxRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.DisplayName);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.Alias);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.PrimarySmtpAddress);

            if (!AliasPattern.IsMatch(request.Alias))
            {
                throw new ArgumentException(
                    "Alias contains characters that Exchange Online does not accept.",
                    nameof(request.Alias));
            }

            try
            {
                _ = new MailAddress(request.PrimarySmtpAddress);
            }
            catch (FormatException exception)
            {
                throw new ArgumentException(
                    "PrimarySmtpAddress must be a valid email address.",
                    nameof(request.PrimarySmtpAddress),
                    exception);
            }
        }
    }
}
