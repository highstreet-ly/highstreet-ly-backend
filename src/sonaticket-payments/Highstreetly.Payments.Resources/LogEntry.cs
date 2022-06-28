using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Payments.Resources
{
    [Table("logs", Schema = "public")]
    [Resource("payments-logs")]
    public class LogEntry : Identifiable<int>
    {

        [Attr]
        [Column("message")]
        public string Message { get; set; }

        [Attr]
        [Column("exception")]
        public string Exception { get; set; }

        [Attr]
        [Column("level")]
        public string Level { get; set; }

        [Attr]
        [Column("raise_date")]
        public DateTime RaiseDate { get; set; }

        [Attr]
        [Column("properties", TypeName = "jsonb")]
        public string Properties { get; set; }
    }
}