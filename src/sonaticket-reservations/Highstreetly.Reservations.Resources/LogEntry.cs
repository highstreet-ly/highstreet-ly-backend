using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Reservations.Resources
{
    [Table("logs", Schema = "public")]
    [Resource("reservations-logs")]
    public class LogEntry : Identifiable<int>
    {

        [Column("message")]
        [Attr]
        public string Message { get; set; }

        [Attr]
        [Column("exception")]
        public string Exception { get; set; }

        [Column("level")]
        [Attr]
        public string Level { get; set; }

        [Column("raise_date")]
        [Attr]
        public DateTime RaiseDate { get; set; }

        [Column("properties", TypeName = "jsonb")]
        [Attr]
        public string Properties { get; set; }
    }
}