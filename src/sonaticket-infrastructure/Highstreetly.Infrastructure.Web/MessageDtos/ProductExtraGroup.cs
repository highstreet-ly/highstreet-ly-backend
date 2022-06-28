using System;
using System.Collections.Generic;

namespace Highstreetly.Infrastructure.MessageDtos
{
    public class ProductExtraGroup
    {
        public Guid Id { get; set; }

        public List<ProductExtra> ProductExtras { get; set; }
        
        public int MaxSelectable { get; set; }
        
        public int MinSelectable { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
    }
}