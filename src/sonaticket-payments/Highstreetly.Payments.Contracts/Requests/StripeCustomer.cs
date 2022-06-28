using System;
using Highstreetly.Infrastructure;
using Newtonsoft.Json;

namespace Highstreetly.Payments.Contracts.Requests
{
    [JsonObject(Title = "stripe-customers")]
    [RequestScope(Scope = Scopes.PaymentApi)]
    [RequestService(Service = Services.PaymentApi)]
    public class StripeCustomer
    {
        public string Type => "stripe-customers";

        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("account-balance")] public int? AccountBalance { get; set; }
        [JsonProperty("currency")] public string Currency { get; set; }
        [JsonProperty("default-source")] public string DefaultSource { get; set; }
        [JsonProperty("delinquent")] public bool Delinquent { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("discount")] public string Discount { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("live-mode")] public bool Livemode { get; set; }
        [JsonProperty("shipping")] public string Shipping { get; set; }
        [JsonProperty("sonatrbe-user-id")] public Guid SonaticketUserId { get; set; }
    }
}