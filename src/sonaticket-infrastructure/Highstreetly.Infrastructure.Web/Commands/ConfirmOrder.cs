using System;

namespace Highstreetly.Infrastructure.Commands
{
    public class ConfirmOrder : IConfirmOrder
    {
        public ConfirmOrder()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
       public Guid CorrelationId { get; set; }
        public Guid OrderId { get; set; }
        public string Email { get; set; }
    }
}