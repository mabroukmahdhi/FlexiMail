// ---------------------------------------
// Copyright (c) 2024 Mabrouk Mahdhi.
// Made with love for the .NET Community
// ---------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Inbounds;
using FlexiMail.Models.Foundations.Mailboxes;
using FlexiMail.Models.Foundations.Subscriptions;

namespace FlexiMail.Tests.Manual
{
    internal static class Program
    {
        private static readonly GraphMailConfigurations Configurations = new()
        {
            TenantId = "",
            ClientId = "",
            ClientSecret = "",
            SenderUserIdOrUpn = "",
            Scopes = ["https://graph.microsoft.com/.default"]
        };

        private const string NotificationUrl =
            "https://your-public-host.example/webhooks/fleximail";

        private const string LifecycleNotificationUrl =
            "https://your-public-host.example/webhooks/fleximail/lifecycle";

        private const string ClientState =
            "REPLACE_WITH_A_LONG_RANDOM_SECRET";

        private static readonly IFlexiMailClient Client =
            new FlexiMailClient(Configurations);

        private static readonly ExchangeProvisioningConfigurations ProvisioningConfigurations = new()
        {
            AppId = "",
            Organization = "",
            CertificateThumbprint = "",
            PowerShellExecutable = "pwsh"
        };

        private static FlexiMailSubscription currentSubscription;

        private static async Task Main()
        {
            Console.WriteLine("FlexiMail Microsoft Graph manual tests");
            Console.WriteLine("Replace the placeholders in Program.cs before running.");

            while (true)
            {
                WriteMenu();
                var choice = Console.ReadLine()?.Trim();

                if (choice == "0")
                {
                    return;
                }

                try
                {
                    await RunScenarioAsync(choice);
                }
                catch (Exception exception)
                {
                    WriteException(exception);
                }
            }
        }

        private static void WriteMenu()
        {
            Console.WriteLine();
            Console.WriteLine("1 - List the 10 newest Inbox messages");
            Console.WriteLine("2 - List the 10 unread Inbox messages");
            Console.WriteLine("3 - Read one message by ID");
            Console.WriteLine("4 - Create a new-message subscription");
            Console.WriteLine("5 - Renew a subscription");
            Console.WriteLine("6 - Delete a subscription");
            Console.WriteLine("7 - Create an Exchange Online shared mailbox");
            Console.WriteLine("0 - Exit");
            Console.Write("Select a scenario: ");
        }

        private static Task RunScenarioAsync(string choice) => choice switch
        {
            "1" => ListInboxAsync(unreadOnly: false),
            "2" => ListInboxAsync(unreadOnly: true),
            "3" => ReadMessageAsync(),
            "4" => CreateSubscriptionAsync(),
            "5" => RenewSubscriptionAsync(),
            "6" => DeleteSubscriptionAsync(),
            "7" => CreateSharedMailboxAsync(),
            _ => Task.Run(() => Console.WriteLine("Unknown selection."))
        };

        private static async Task ListInboxAsync(bool unreadOnly)
        {
            FlexiReceivedMessagePage page = await Client.GetInboxAsync(
                mailbox: Configurations.SenderUserIdOrUpn,
                pageSize: 10,
                unreadOnly: unreadOnly);

            Console.WriteLine();
            Console.WriteLine($"Returned {page.Messages.Count} message(s).");

            foreach (var message in page.Messages)
            {
                Console.WriteLine(
                    $"[{message.ReceivedDateTime:u}] " +
                    $"Read={message.IsRead,-5} " +
                    $"From={message.From} " +
                    $"Subject={message.Subject}");

                Console.WriteLine($"  ID: {message.Id}");
            }

            if (!string.IsNullOrWhiteSpace(page.NextLink))
            {
                Console.WriteLine("More messages are available in Microsoft Graph.");
            }
        }

        private static async Task ReadMessageAsync()
        {
            Console.Write("Message ID (leave empty to use the newest message): ");
            var messageId = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(messageId))
            {
                var page = await Client.GetInboxAsync(
                    Configurations.SenderUserIdOrUpn,
                    pageSize: 1);

                messageId = page.Messages.FirstOrDefault()?.Id;
            }

            if (string.IsNullOrWhiteSpace(messageId))
            {
                Console.WriteLine("No message was found.");
                return;
            }

            var message = await Client.GetReceivedMessageAsync(
                messageId,
                Configurations.SenderUserIdOrUpn);

            Console.WriteLine();
            Console.WriteLine($"ID:       {message.Id}");
            Console.WriteLine($"From:     {message.From}");
            Console.WriteLine($"To:       {string.Join(", ", message.To ?? [])}");
            Console.WriteLine($"Cc:       {string.Join(", ", message.Cc ?? [])}");
            Console.WriteLine($"Received: {message.ReceivedDateTime:u}");
            Console.WriteLine($"Subject:  {message.Subject}");
            Console.WriteLine($"Read:     {message.IsRead}");
            Console.WriteLine($"Body:     {message.Body?.Content}");

            foreach (var attachment in message.Attachments ?? [])
            {
                Console.WriteLine(
                    $"Attachment: {attachment.Name} ({attachment.Bytes?.Length ?? 0} bytes)");
            }
        }

        private static async Task CreateSubscriptionAsync()
        {
            currentSubscription = await Client.SubscribeToInboxAsync(
                notificationUrl: NotificationUrl,
                clientState: ClientState,
                mailbox: Configurations.SenderUserIdOrUpn,
                lifecycleNotificationUrl: LifecycleNotificationUrl);

            PrintSubscription("Created", currentSubscription);
        }

        private static async Task RenewSubscriptionAsync()
        {
            var subscriptionId = ReadSubscriptionId();

            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                return;
            }

            currentSubscription = await Client.RenewSubscriptionAsync(subscriptionId);
            PrintSubscription("Renewed", currentSubscription);
        }

        private static async Task DeleteSubscriptionAsync()
        {
            var subscriptionId = ReadSubscriptionId();

            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                return;
            }

            await Client.DeleteSubscriptionAsync(subscriptionId);
            currentSubscription = null;
            Console.WriteLine($"Deleted subscription {subscriptionId}.");
        }

        private static string ReadSubscriptionId()
        {
            Console.Write(
                "Subscription ID" +
                (currentSubscription is null ? string.Empty : " (leave empty to use the current ID)") +
                ": ");

            var subscriptionId = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                subscriptionId = currentSubscription?.Id;
            }

            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                Console.WriteLine("A subscription ID is required.");
            }

            return subscriptionId;
        }

        private static async Task CreateSharedMailboxAsync()
        {
            Console.Write("Display name: ");
            var displayName = Console.ReadLine()?.Trim();
            Console.Write("Alias (for example, support): ");
            var alias = Console.ReadLine()?.Trim();
            Console.Write("Primary SMTP address: ");
            var primarySmtpAddress = Console.ReadLine()?.Trim();

            Console.Write(
                $"Type CREATE to provision shared mailbox '{primarySmtpAddress}': ");

            if (!string.Equals(Console.ReadLine()?.Trim(), "CREATE", StringComparison.Ordinal))
            {
                Console.WriteLine("Creation cancelled.");
                return;
            }

            IFlexiMailboxProvisioningClient provisioningClient =
                new FlexiMailboxProvisioningClient(ProvisioningConfigurations);

            var mailbox = await provisioningClient.CreateSharedMailboxAsync(
                new FlexiSharedMailboxRequest
                {
                    DisplayName = displayName,
                    Alias = alias,
                    PrimarySmtpAddress = primarySmtpAddress
                });

            Console.WriteLine("Created shared mailbox:");
            Console.WriteLine($"  Identity: {mailbox.Identity}");
            Console.WriteLine($"  Object ID: {mailbox.ExternalDirectoryObjectId}");
            Console.WriteLine($"  Address: {mailbox.PrimarySmtpAddress}");
            Console.WriteLine($"  Type: {mailbox.RecipientTypeDetails}");
        }

        private static void PrintSubscription(
            string operation,
            FlexiMailSubscription subscription)
        {
            Console.WriteLine($"{operation} subscription:");
            Console.WriteLine($"  ID:       {subscription.Id}");
            Console.WriteLine($"  Resource: {subscription.Resource}");
            Console.WriteLine($"  Expires:  {subscription.ExpirationDateTime:u}");
        }

        private static void WriteException(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine($"{exception.GetType().Name}: {exception.Message}");

            if (exception.InnerException is not null)
            {
                Console.WriteLine($"Inner: {exception.InnerException.Message}");
            }

            Console.ResetColor();
        }
    }
}
