using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Resource("dashboard-stats")]
    [Table("DashboardStat", Schema = "Management")]
    public class DashboardStat : Identifiable<Guid>
    {
        public DashboardStat()
        {
            TicketsSoldByDay = new List<TicketsSoldByDay>();
            OrdersProcessedByDay = new List<OrdersByDay>();
            RegisteredInterestByDay = new List<RegisteredInterestByDay>();
            RefundsProcessedByDay = new List<RefundsByDay>();
        }

        [HasMany]
        public List<TicketsSoldByDay> TicketsSoldByDay { get; set; }

        [HasMany]
        public List<RegisteredInterestByDay> RegisteredInterestByDay { get; set; }

        [HasMany]
        public List<OrdersByDay> OrdersProcessedByDay { get; set; }

        [HasMany]
        public List<RefundsByDay> RefundsProcessedByDay { get; set; }

        [Attr]
        [NotMapped]
        public long FundsAllTime
        {
            get
            {
                if (TicketsSoldByDay == null)
                {
                    return 0;
                }

                return TicketsSoldByDay.Sum(x => x.TotalFunds);
            }
        }

        [Attr]
        [NotMapped]
        public int RegisteredInterest
        {
            get
            {
                if (RegisteredInterestByDay == null)
                {
                    return 0;
                }

                return RegisteredInterestByDay.Sum(x => x.Total);
            }
        }

        [Attr]
        [NotMapped]
        public int TicketsSoldAllTime
        {
            get
            {
                if (TicketsSoldByDay == null)
                {
                    return 0;
                }

                return TicketsSoldByDay.Sum(x => x.Total);
            }
        }

        [Attr]
        [NotMapped]
        public int OrdersProcessedAllTime
        {
            get
            {
                if (OrdersProcessedByDay == null)
                {
                    return 0;
                }

                return OrdersProcessedByDay.Sum(x => x.Total);
            }
        }

        [Attr]
        [NotMapped]
        public int TotalOrdersCreated
        {
            get
            {
                if (OrdersProcessedByDay == null)
                {
                    return 0;
                }

                return OrdersProcessedByDay.Sum(x => x.Total);
            }
        }

        [Attr]
        public int TotalOrdersFullfiled { get; set; }

        [Attr]
        public int TotalOrdersAbandoned { get; set; }

        [Attr]
        public Guid EventOrganiserId { get; set; }
        
        [HasOne]
        public EventOrganiser EventOrganiser { get; set; }
    }
}