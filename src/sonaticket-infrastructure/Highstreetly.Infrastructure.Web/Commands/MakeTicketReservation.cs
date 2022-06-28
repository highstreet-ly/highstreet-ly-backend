using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Commands
{
    public class MakeTicketReservation : IMakeTicketReservation
    {
        public MakeTicketReservation()
        {
            Tickets = new List<TicketQuantity>();
        }

        public Guid ReservationId { get; set; }
       
        public Guid Id { get; set; }
        
        public TimeSpan Delay { get; set; }
        
        public string TypeInfo { get; set; }
        
        public Guid CorrelationId { get; set; }
        
        public Guid EventInstanceId { get; set; }
        
        public string SessionId { get; }
        
        public List<TicketQuantity> Tickets { get; set; }
        
        public bool IsStockManaged { get; set; }
    }
}