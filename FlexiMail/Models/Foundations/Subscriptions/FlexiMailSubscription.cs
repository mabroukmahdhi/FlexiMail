using System;

namespace FlexiMail.Models.Foundations.Subscriptions
{
    /// <summary>
    /// Represents a Microsoft Graph change-notification subscription for mail.
    /// </summary>
    public class FlexiMailSubscription
    {
        /// <summary>Gets or sets the subscription identifier.</summary>
        public string Id { get; set; }
        /// <summary>Gets or sets the Microsoft Graph resource monitored by the subscription.</summary>
        public string Resource { get; set; }
        /// <summary>Gets or sets the subscribed change type.</summary>
        public string ChangeType { get; set; }
        /// <summary>Gets or sets the endpoint that receives resource notifications.</summary>
        public string NotificationUrl { get; set; }
        /// <summary>Gets or sets the endpoint that receives subscription lifecycle notifications.</summary>
        public string LifecycleNotificationUrl { get; set; }
        /// <summary>Gets or sets the UTC date and time at which the subscription expires.</summary>
        public DateTimeOffset? ExpirationDateTime { get; set; }
    }
}
