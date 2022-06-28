using System;
using System.Collections.Generic;
using Highstreetly.Management.Contracts;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using Highstreetly.Infrastructure;
using Newtonsoft.Json;

namespace Highstreetly.Management.Resources
{
    [Table("Order", Schema = "Management")]
    [Resource("orders")]
    public class Order : Identifiable<Guid>, IHasResourceMetadata
    {
        public Order()
        {
            Tickets = new List<OrderTicket>();
            Metadata = new Dictionary<string, string>();
        }
        
        [Attr]
        public Guid EventInstanceId { get; set; }

        [Attr]
        public Guid? OwnerId { get; set; }

        [Attr]
        public string OwnerEmail { get; set; }

        [Attr]
        public long TotalAmount { get; set; }

        [Attr]
        public OrderStatus Status { get; set; }

        [HasMany]
        public ICollection<OrderTicket> Tickets { get; set; }
        //
        // [Attr]
        // public ICollection<OrderPayment> Payments { get; set; }

        [Attr]
        public Guid RegistrantUserId { get; set; }

        [Attr]
        public DateTime CreatedOn { get; set; }

        [Attr]
        public DateTime ConfirmedOn { get; set; }

        [Attr]
        public string HumanReadableId { get; set; }

        [Attr]
        public bool IsClickAndCollect { get; set; }

        [Attr]
        public bool IsLocalDelivery { get; set; }

        [Attr]
        public bool? IsToTable { get; set; }

        [Attr]
        public string TableInfo { get; set; }

        [Attr]
        public bool? MakeSubscription { get; set; }

        [Attr]
        public bool IsNationalDelivery { get; set; }
        
        [Attr]
        public bool Refunded { get; set; }

        [Attr]
        public string OwnerPhone { get; set; }

        [Attr]
        public string OwnerName { get; set; }

        [Attr]
        public string DeliveryLine1 { get; set; }

        [Attr]
        public string DeliveryPostcode { get; set; }

        [Attr]
        public DateTime RefundedDateTime { get; set; }

        [Attr]
        public string RefundedReason { get; set; }

        [Attr]
        public string CustomerDispatchAdvisory { get; set; }

        [Attr] public DateTime? OrderAdvisoryDate { get; set; }

        [Attr] public string OrderAdvisoryTimeOfDay { get; set; }

        [Attr]
        public DateTime PricedDateTime { get; set; }

        [Attr]
        public DateTime PaidDateTime { get; set; }

        [Attr]
        public DateTime ProcessingDateTime { get; set; }

        [Attr]
        public DateTime ProcessingCompleteDateTime { get; set; }

        [Attr]
        public DateTime ExpiredDateTime { get; set; }

        [Column("Metadata", TypeName = "jsonb")]
        public string MetadataDB { get; set; }

        [NotMapped]
        [Attr]
        public Dictionary<string, string> Metadata
        {
            get => !string.IsNullOrWhiteSpace(MetadataDB) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(MetadataDB) : new Dictionary<string, string>();
            set => MetadataDB = JsonConvert.SerializeObject(value);
        }

        [Attr]
        public Guid? PaymentId { get; set; }

        [Attr]
        public long PaymentPlatformFees { get; set; }

        [Attr]
        public long PlatformFees { get; set; }
    }
}