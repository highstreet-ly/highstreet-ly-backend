using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace Highstreetly.Infrastructure.MessageDtos
{
    public class OrderTicketDetails
    {
        public OrderTicketDetails()
        {
            ProductExtras = new List<ProductExtra>();
        }

        public Guid Id { get; set; }

        public Guid EventInstanceId { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public long Price { get; set; }

        public int Quantity { get; set; }

        public List<ProductExtraGroup> ProductExtraGroups { get; set; }

        public List<ProductExtra> ProductExtras { get; set; }

        [NotMapped, JsonIgnore]
        public string ExtrasComparer
        {
            get
            {
                return $"{this.Id}-{string.Join(",", ProductExtras.Select(x => x.Name))}";
            }
        }
    }
}