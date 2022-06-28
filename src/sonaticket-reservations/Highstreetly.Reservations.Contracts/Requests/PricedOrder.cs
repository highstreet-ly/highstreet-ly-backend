using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure;
using Newtonsoft.Json;

namespace Highstreetly.Reservations.Contracts.Requests
{
    [JsonObject(Title = "priced-orders")]
    [RequestScope(Scope = Scopes.TicketReservationsApi)]
    [RequestService(Service = Services.TicketReservationsApi)]
    public class PricedOrder
    {
        [JsonProperty("type")] 
        public string Type => "priced-orders";
        
        [JsonProperty("order-id")] 
        public Guid OrderId { get; set; }
        
        [JsonProperty("id")] 
        public Guid Id { get; set; }
        
        [JsonProperty("assignments-id")] 
        public Guid? AssignmentsId { get; set; }
        
        [JsonProperty("priced-order-lines")]
        public IList<PricedOrderLine> PricedOrderLines { get; set; }
        
        [JsonProperty("total")] 
        public long? Total { get; set; }
        
        [JsonProperty("order-version")] 
        public int? OrderVersion { get; set; }
        
        [JsonProperty("is-free-of-charge")]
        public bool IsFreeOfCharge { get; set; }
        
        [JsonProperty("reservation-expiration-date")]
        public DateTime? ReservationExpirationDate { get; set; }
        
        [JsonProperty("owner-id")] 
        public string OwnerId { get; set; }
        
        [JsonProperty("human-readable-id")] 
        public string HumanReadableId { get; set; }
        
        [JsonProperty("payment-platform-fees")] 
        public long? PaymentPlatformFees { get; set; }
        
        [JsonProperty("platform-fees")]
        public long? PlatformFees { get; set; }
        
        [JsonProperty("delivery-fee")] 
        public long? DeliveryFee { get; set; }

        [JsonProperty("make-subscription")]
        public bool? MakeSubscription { get; set; }

        [JsonProperty("is-to-table")]
        public bool? IsToTable { get; set; }

        [JsonProperty("table-info")]
        public string TableInfo { get; set; }
    }
}
