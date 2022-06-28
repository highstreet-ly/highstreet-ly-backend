using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Highstreetly.Infrastructure;
using Highstreetly.Reservations.Contracts.Requests;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using Newtonsoft.Json;

namespace Highstreetly.Reservations.Resources
{
    [Resource("draft-orders")]
    [Table("DraftOrder", Schema = "Reservation")]
    public class DraftOrder : Identifiable<Guid>, IHasResourceMetadata, IHasOwner
    {
        public DraftOrder()
        {
            DraftOrderItems = new List<DraftOrderItem>();
            Metadata = new Dictionary<string, string>();
        }

        [Attr]
        public Guid OrderId
        {
            get => Id;
            set => Id = value;
        }

        [Attr] 
        public Guid EventInstanceId { get; set; }

        [Attr]
        public DateTime? ReservationExpirationDate { get; set; }

        [HasMany(PublicName = "draft-order-items")] 
        public List<DraftOrderItem> DraftOrderItems { get; set; }

        [Attr] 
        public States State { get; set; }

        [Attr] 
        public int OrderVersion { get; set; }
        
        [Attr] 
        public Guid? OwnerId { get; set; }
        
        [Attr] 
        public string OwnerEmail { get; set; }
        
        [Attr] 
        public string OwnerPhone { get; set; }
        
        [Attr] 
        public string OwnerName { get; set; }
        
        [Attr] 
        public string DeliveryLine1 { get; set; }
        
        [Attr] 
        public string DeliveryPostcode { get; set; }
        
        [Attr] 
        [NotMapped] public string UserToken { get; set; }
        
        [Attr] 
        public string HumanReadableId { get; set; }

        [Attr]
        public bool? IsToTable { get; set; }

        [Attr]
        public string TableInfo { get; set; }

        [Attr] 
        public bool IsClickAndCollect { get; set; }
        
        [Attr] 
        public bool IsLocalDelivery { get; set; }

        [Attr]
        public bool? MakeSubscription { get; set; }

        [Attr] 
        public bool IsNationalDelivery { get; set; }

        [Column("Metadata", TypeName = "jsonb")]
        public string MetadataDB { get; set; }

        [NotMapped ,Attr]
        public Dictionary<string, string> Metadata
        {
            get => !string.IsNullOrWhiteSpace(MetadataDB) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(MetadataDB) : new Dictionary<string, string>();
            set => MetadataDB = JsonConvert.SerializeObject(value);
        }
    }
}