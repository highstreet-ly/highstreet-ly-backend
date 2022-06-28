using System;
using Highstreetly.Infrastructure;
using Newtonsoft.Json;

namespace Highstreetly.Management.Contracts.Requests
{
    [JsonObject(Title = "orders")]
    [RequestScope(Scope = Scopes.TicketManagementApi)]
    [RequestService(Service = Services.TicketManagementApi)]
    public class Order
    {
        public string Type => "orders";

        [JsonProperty(propertyName: "order-id")]
        public Guid OrderId
        {
            get => Id;
            set => Id = value;
        }

        [JsonProperty(propertyName: "id")] 
        public Guid Id { get; set; }

        [JsonProperty(propertyName: "event-instance-id")]
        public Guid EventInstanceId { get; set; }

        [JsonProperty("refunded-reason")]
        public string RefundedReason { get; set; }

        [JsonProperty("is-click-and-collect")]
        public bool IsClickAndCollect { get; set; }

        [JsonProperty("is-local-delivery")]
        public bool IsLocalDelivery { get; set; }

        [JsonProperty("is-national-delivery")]
        public bool IsNationalDelivery { get; set; }

        [JsonProperty("customer-dispatch-advisory")]
        public string CustomerDispatchAdvisory { get; set; }

        [JsonProperty("make-subscription")]
        public bool? MakeSubscription { get; set; }

        [JsonProperty("is-to-table")]
        public bool? IsToTable { get; set; }

        [JsonProperty("table-info")]
        public string TableInfo { get; set; }
        
        [JsonProperty("refunded")]
        public bool Refunded { get; set; }
    }
}