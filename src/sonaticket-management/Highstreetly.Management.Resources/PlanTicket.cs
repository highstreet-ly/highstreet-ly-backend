using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("PlanTickets", Schema = "Management")]
    public class PlanTicket
    {
        public Guid PlanId { get; set; }

        [HasOne]
        public Plan Plan { get; set; }

        public Guid TicketTypeId { get; set; }

        [HasOne]
        public TicketType TicketType { get; set; }
    }
}