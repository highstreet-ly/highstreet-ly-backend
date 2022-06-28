using System;
using Highstreetly.Infrastructure;
using Newtonsoft.Json;

namespace Highstreetly.Management.Contracts.Requests
{
    [JsonObject(Title = "event-instances")]
    [RequestScope(Scope = Scopes.TicketManagementApi)]
    [RequestService(Service = Services.TicketManagementApi)]
    public class EventInstance
    {
        public string Type => "event-instances";

        [JsonProperty(propertyName: "id")]
        public Guid Id { get; set; }

        [JsonProperty(propertyName: "owner-id")]
        public string OwnerId { get; set; }

        [JsonProperty(propertyName: "event-series-id")]
        public Guid EventSeriesId { get; set; }

        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }

        [JsonProperty("normalized-name")]
        public string NormalizedName { get; set; }

        [JsonProperty(propertyName: "short-location")]
        public string ShortLocation { get; set; }

        [JsonProperty("postcode")]
        public string PostCode { get; set; }

        [JsonProperty("delivery-radius-miles")]
        public int? DeliveryRadiusMiles { get; set; }

        [JsonProperty(propertyName: "tagline")]
        public string Tagline { get; set; }

        [JsonProperty(propertyName: "main-image-id")]
        public string MainImageId { get; set; }

        [JsonProperty(propertyName: "slug")]
        public string Slug { get; set; }

        [JsonProperty(propertyName: "start-date")]
        public DateTime StartDate { get; set; }

        [JsonProperty(propertyName: "is-published")]
        public bool IsPublished { get; set; }

        [JsonProperty(propertyName: "end-date")]
        public DateTime EndDate { get; set; }

        [JsonProperty("event-organiser-id")]
        public Guid EventOrganiserId { get; set; }

        [JsonProperty("event-organiser")]
        public EventOrganiser EventOrganiser { get; set; }

        [JsonProperty("is-click-and-collect")]
        public bool IsClickAndCollect { get; set; }

        [JsonProperty("is-local-delivery")]
        public bool IsLocalDelivery { get; set; }

        [JsonProperty("is-national-delivery")]
        public bool IsNationalDelivery { get; set; }

        [JsonProperty("national-delivery-flat-fee")]
        public long? NationalDeliveryFlatFee { get; set; }

        [JsonProperty("national-delivery-flat-fee-free-after")]
        public long? NationalDeliveryFlatFeeFreeAfter { get; set; }

        public EventSeries EventSeries { get; set; }
        
        [JsonProperty("payment-platform-fee-paid-by")]
        public int? PaymentPlatformFeePaidBy { get; set; }
        
        [JsonProperty("platform-fee-paid-by")]
        public int? PlatformFeePaidBy { get; set; }

        [JsonProperty("is-to-table")]
        public bool? IsToTable { get; set; }
        
        [JsonProperty("is-stock-managed")]
        public bool IsStockManaged { get; set; }

        [JsonProperty("business-type-id")]
        public Guid? BusinessTypeId { get; set; }
    }
}
