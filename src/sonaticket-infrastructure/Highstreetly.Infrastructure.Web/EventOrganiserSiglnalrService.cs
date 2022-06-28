using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Signalr;
using MassTransit;
using MassTransit.SignalR.Contracts;
using MassTransit.SignalR.Utils;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Infrastructure
{
    public class EventOrganiserSiglnalrService : IEventOrganiserSiglnalrService
    {
        private readonly IBusControl _busControl;
        private ILogger<EventOrganiserSiglnalrService> _logger;
        readonly IReadOnlyList<IHubProtocol> _protocols = new IHubProtocol[] { new JsonHubProtocol() };

        public EventOrganiserSiglnalrService(IBusControl busControl, ILogger<EventOrganiserSiglnalrService> logger)
        {
            _busControl = busControl;
            _logger = logger;
        }

        public async Task Send(string orgId, string message)
        {
            _logger.LogInformation($"Sending message to group {orgId}");
            
            await _busControl.Publish<Group<ChatHub>>(new
            {
                GroupName = orgId,
                Messages = _protocols.ToProtocolDictionary("broadcastMessage", new object[] { "backend-process", message })
            });
        }
    }
}