using Microsoft.AspNetCore.SignalR;

namespace Highstreetly.Signalr
{
    public class ChatHub : Hub
    {
        public void Send(string group, string name, string message)
        {
            Clients.Group(group).SendAsync("broadcastMessage", name, message);
        }

        public void JoinGroup(string groupName)
        {
            Groups.AddToGroupAsync(this.Context.ConnectionId, groupName).ConfigureAwait(false);
        }
    }
}