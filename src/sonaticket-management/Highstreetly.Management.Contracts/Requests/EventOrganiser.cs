using System;
using Highstreetly.Infrastructure;
using Newtonsoft.Json;

namespace Highstreetly.Management.Contracts.Requests
{
    [JsonObject(Title = "event-organisers")]
    [RequestScope(Scope = Scopes.TicketManagementApi)]
    [RequestService(Service = Services.TicketManagementApi)]
    public class EventOrganiser
    {
        public string Type => "event-organisers";

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("stripe-account-id")]
        public string StripeAccountId { get; set; }

        [JsonProperty("stripe-publishable-key")]
        public string StripePublishableKey { get; set; }

        [JsonProperty("stripe-access-token")]
        public string StripeAccessToken { get; set; }

        [JsonProperty("is-connected-to-stripe")]
        public bool IsConnectedToStripe
        {
            get =>
                !string.IsNullOrEmpty(StripeAccountId) && !string.IsNullOrEmpty(StripePublishableKey) &&
                !string.IsNullOrEmpty(StripeAccessToken);
            set
            {
                var x = value;
            }
        }

        [JsonProperty("stripe-login-link")]
        public string StripeLoginLink { get; set; }

        [JsonProperty("string-id")]
        public string StringId { get => Id.ToString(); set => Id = Guid.Parse(value); }

        [JsonProperty("stripe-code")]
        public string StripeCode { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("normalized-name")]
        public string NormalizedName { get; set; }

        [JsonProperty("logo-id")]
        public string LogoId { get; set; }

        [JsonProperty("platform-fee")]
        public long? PlatformFee { get; set; }
    }
}