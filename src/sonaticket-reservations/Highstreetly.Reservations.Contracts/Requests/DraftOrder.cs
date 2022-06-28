using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure;
using Newtonsoft.Json;

namespace Highstreetly.Reservations.Contracts.Requests
{
    [JsonObject(Title = "draft-orders")]
    [RequestScope(Scope = Scopes.TicketReservationsApi)]
    [RequestService(Service = Services.TicketReservationsApi)]
    public class DraftOrder
    {
        public string Type => "draft-orders";

        [JsonProperty(propertyName: "order-id")]
        public Guid OrderId { get => Id; set => Id = value; }

        [JsonProperty(propertyName: "id")]
        public Guid Id { get; set; }

        [JsonProperty(propertyName: "event-instance-id")]
        public Guid EventInstanceId { get; set; }

        [JsonProperty(propertyName: "reservation-expiration-date")]
        public DateTime? ReservationExpirationDate { get; set; }

        [JsonProperty(propertyName: "draft-order-items")]
        public IList<DraftOrderItem> DraftOrderItems { get; set; }

        [JsonProperty(propertyName: "state")]
        public States State { get; set; }

        // [JsonProperty(propertyName: "state-value")]
        // public int StateValue { get => (int)State; set { } }

        [JsonProperty(propertyName: "order-version")]
        public int? OrderVersion { get; set; }

        [JsonProperty(propertyName: "owner-id")]
        public Guid? OwnerId { get; set; }

        [JsonProperty(propertyName: "owner-email")]
        public string OwnerEmail { get; set; }
        [JsonProperty("is-click-and-collect")]
        public bool IsClickAndCollect { get; set; }

        [JsonProperty("is-local-delivery")]
        public bool IsLocalDelivery { get; set; }

        [JsonProperty("is-national-delivery")]
        public bool IsNationalDelivery { get; set; }
        
        [JsonProperty("make-subscription")]
        public bool? MakeSubscription { get; set; }

        [JsonProperty("owner-phone")]
        public string OwnerPhone { get; set; }

        [JsonProperty("owner-name")]
        public string OwnerName { get; set; }

        [JsonProperty("delivery-line1")]
        public string DeliveryLine1 { get; set; }

        [JsonProperty("delivery-postcode")]
        public string DeliveryPostcode { get; set; }

        [JsonProperty("is-to-table")]
        public bool? IsToTable { get; set; }

        [JsonProperty("table-info")] 
        public string TableInfo { get; set; }
    }
}