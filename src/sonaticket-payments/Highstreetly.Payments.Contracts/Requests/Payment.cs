using System;
using Highstreetly.Infrastructure;
using Newtonsoft.Json;

namespace Highstreetly.Payments.Contracts.Requests
{
    [JsonObject(Title = "payments")]
    [RequestScope(Scope = Scopes.PaymentApi)]
    [RequestService(Service = Services.PaymentApi)]
    public class Payment
    {
        public string Type => "payments";
        [JsonProperty(propertyName: "payment-intent-id")] 
        public string PaymentIntentId { get; set; }
        [JsonProperty(propertyName: "token")] 
        public string Token { get; set; }
        [JsonProperty(propertyName: "email")] 
        public string Email { get; set; }
        [JsonProperty(propertyName: "event-instance-id")]
        public Guid EventInstanceId { get; set; }
        [JsonProperty(propertyName: "order-id")] 
        public Guid OrderId { get; set; }
        [JsonProperty(propertyName: "order-version")] 
        public int? OrderVersion { get; set; }
        [JsonProperty(propertyName: "payment-id")] 
        public Guid PaymentId { get => Id; set => Id = value; }
        [JsonProperty(propertyName: "id")]
        public Guid Id { get; set; }
    }
}