using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Management.ReadModel
{
    public interface ITicketQuantityService
    {
        Task UpdateAvailableQuantity(ISonaticketEvent @event, IEnumerable<TicketQuantity> seats);
    }
}