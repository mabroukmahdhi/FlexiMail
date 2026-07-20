using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FlexiMail.Models.Foundations.Subscriptions
{
    /// <summary>
    /// Represents a collection of Microsoft Graph mail change notifications.
    /// </summary>
    public class FlexiMailNotificationCollection
    {
        /// <summary>Gets or sets the notifications delivered in the webhook request.</summary>
        [JsonPropertyName("value")]
        public List<FlexiMailNotification> Value { get; set; } = [];
    }

    /// <summary>
    /// Represents a Microsoft Graph notification for a mail resource change.
    /// </summary>
    public class FlexiMailNotification
    {
        /// <summary>Gets or sets the identifier of the originating subscription.</summary>
        [JsonPropertyName("subscriptionId")]
        public string SubscriptionId { get; set; }

        /// <summary>Gets or sets the expiration date and time of the originating subscription.</summary>
        [JsonPropertyName("subscriptionExpirationDateTime")]
        public DateTimeOffset? SubscriptionExpirationDateTime { get; set; }

        /// <summary>Gets or sets the secret state supplied when the subscription was created.</summary>
        [JsonPropertyName("clientState")]
        public string ClientState { get; set; }

        /// <summary>Gets or sets the type of resource change.</summary>
        [JsonPropertyName("changeType")]
        public string ChangeType { get; set; }

        /// <summary>Gets or sets the path of the changed Microsoft Graph resource.</summary>
        [JsonPropertyName("resource")]
        public string Resource { get; set; }

        /// <summary>Gets or sets identity data for the changed resource.</summary>
        [JsonPropertyName("resourceData")]
        public FlexiMailNotificationResourceData ResourceData { get; set; }

        /// <summary>Determines whether this notification contains the expected client state.</summary>
        /// <param name="expectedClientState">The secret state originally used to create the subscription.</param>
        /// <returns><see langword="true"/> when the states match; otherwise, <see langword="false"/>.</returns>
        public bool HasClientState(string expectedClientState) =>
            !string.IsNullOrEmpty(expectedClientState) &&
            string.Equals(ClientState, expectedClientState, StringComparison.Ordinal);
    }

    /// <summary>
    /// Represents identifying data for the mail resource referenced by a notification.
    /// </summary>
    public class FlexiMailNotificationResourceData
    {
        /// <summary>Gets or sets the Microsoft Graph message identifier.</summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>Gets or sets the OData resource identifier.</summary>
        [JsonPropertyName("@odata.id")]
        public string ODataId { get; set; }

        /// <summary>Gets or sets the OData entity tag.</summary>
        [JsonPropertyName("@odata.etag")]
        public string ODataEtag { get; set; }
    }
}
