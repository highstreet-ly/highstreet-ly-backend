using System;
using System.ComponentModel.DataAnnotations;

namespace Highstreetly.Infrastructure.DataProtection
{
    internal class KeyValuesCollection
    {
        public int Id { get; set; }
        public Guid AppId { get; set; }
        public Guid InstanceId { get; set; }

        [MaxLength(2000)]
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}