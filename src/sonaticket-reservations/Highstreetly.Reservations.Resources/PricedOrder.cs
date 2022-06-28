using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Highstreetly.Infrastructure;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using Newtonsoft.Json;

namespace Highstreetly.Reservations.Resources
{
    [Resource("priced-orders")]
    [Table("PricedOrder", Schema = "Reservation")]
    public class PricedOrder : IIdentifiable<Guid>, IHasResourceMetadata, IHasOwner
    {
        public PricedOrder()
        {
            PricedOrderLines = new List<PricedOrderLine>();
            Metadata = new Dictionary<string, string>();
        }

        [Attr]
        public Guid OrderId { get; set; }

        [Attr]
        public Guid Id { get; set; }

        [NotMapped]
        public string StringId { get => Id.ToString(); set => Id = Guid.Parse(value); }

        [NotMapped] public string LocalId { get; set; }

        [HasMany(PublicName = "priced-order-lines")]
        public IList<PricedOrderLine> PricedOrderLines { get; set; }

        [Attr]
        public long Total { get; set; }

        [Attr]
        public int OrderVersion { get; set; }

        [Attr]
        public bool IsFreeOfCharge { get; set; }

        [Attr]
        public bool IsLocalDelivery { get; set; }

        [Attr]
        public bool IsNationalDelivery { get; set; }

        [Attr]
        public bool? IsToTable { get; set; }

        [Attr]
        public string TableInfo { get; set; }

        [Attr]
        public bool? MakeSubscription { get; set; }

        [Attr]
        public DateTime? ReservationExpirationDate { get; set; }

        [Attr]
        public Guid? OwnerId { get; set; }

        [Attr]
        public long PaymentPlatformFees { get; set; }

        [Attr]
        public long PlatformFees { get; set; }

        [Attr]
        public bool OrderIsPriced { get; set; }

        [Attr]
        public string HumanReadableId { get; set; }

        [Attr]
        public long DeliveryFee { get; set; }

        [Column("Metadata", TypeName = "jsonb")]
        public string MetadataDB { get; set; }

        [Attr]
        [NotMapped]
        public Dictionary<string, string> Metadata
        {
            get => !string.IsNullOrWhiteSpace(MetadataDB) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(MetadataDB) : new Dictionary<string, string>();
            set => MetadataDB = JsonConvert.SerializeObject(value);
        }

        [Attr]
        public bool IsClickAndCollect { get; set; }

        [Attr]
        public Guid? EventInstanceId { get; set; }
    }
}