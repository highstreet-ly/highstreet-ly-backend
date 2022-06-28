using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Signalr;
using MassTransit;
using MassTransit.SignalR.Contracts;
using MassTransit.SignalR.Utils;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace Highstreetly.Infrastructure
{
    public class UserSiglnalrService : IUserSiglnalrService
    {
        private readonly IBusControl _busControl;
        readonly IReadOnlyList<IHubProtocol> _protocols = new IHubProtocol[] { new JsonHubProtocol() };

        public UserSiglnalrService(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public async Task Send(string userId, string message)
        {
            await _busControl.Publish<User<ChatHub>>(new
            {
                userId = userId,
                Messages = _protocols.ToProtocolDictionary("broadcastMessage", new object[] { "backend-process", message })
            });
        }
    }
}