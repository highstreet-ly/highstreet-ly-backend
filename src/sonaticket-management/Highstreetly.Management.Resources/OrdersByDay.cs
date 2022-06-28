using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Resource("orders-by-day")]
    [Table("OrdersByDay", Schema = "Management")]
    public class OrdersByDay : Identifiable<Guid>
    {
        [Attr]
        public int Year { get; set; }

        [Attr]
        public int Month { get; set; }

        [Attr]
        public int Day { get; set; }

        [Attr]
        public int Total { get; set; }

        [Attr]
        public long TotalFunds { get; set; }

        [Attr]
        public Guid EventOrganiserId { get; set; }
        
        [HasOne]
        public EventOrganiser EventOrganiser { get; set; }

        [Attr]
        public Guid EventSeriesId { get; set; }
        
        [HasOne]
        public EventSeries EventSeries { get; set; }
        
        [Attr]
        public Guid DashboardStatId { get; set; }
        
        [HasOne]
        public DashboardStat DashboardStat { get; set; }
        
        [Attr]
        public Guid EventInstanceId { get; set; }
        
        [HasOne]
        public EventInstance EventInstance { get; set; }
    }
}