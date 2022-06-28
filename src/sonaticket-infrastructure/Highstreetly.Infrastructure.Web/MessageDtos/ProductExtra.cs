using System;

namespace Highstreetly.Infrastructure.MessageDtos
{
    public class ProductExtra
    {
        public Guid Id { get; set; }
        
        public Guid ReferenceProductExtraId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public long Price { get; set; }

        public bool Selected { get; set; }

        public long ItemCount { get; set; } = 0;
    }
}