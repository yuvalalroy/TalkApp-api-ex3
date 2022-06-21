using WebApp.Services;
using WebApp.Models;
using Microsoft.AspNetCore.SignalR;
namespace WebApp.Hubs
{
    public class MessagesHub : Hub
    {
        private readonly IDictionary<UserConnect, string> _connections;

        public MessagesHub(IDictionary<UserConnect, string> connections)
        {
            _connections = connections;
        }

        public async Task ConnectClientToChat(UserConnect userConnection)
        {
            _connections[userConnection] = Context.ConnectionId;
        }

        public async Task AddMessage(Message message, string from, string to)
        {
            var connection = _connections.Keys.FirstOrDefault(c => c.UserName.Equals(to) && c.ContactId.Equals(from));
            if (connection == null)
                return;

            var connectionId = _connections[connection];
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", new MessagePost
            {
                id = message.id,
                sent = message.sent,
                date = message.created,
                content = message.content
            });
        }        

        public override async Task OnDisconnectedAsync(Exception e)
        {
            var item = _connections.First(k => k.Value.Equals(Context.ConnectionId));
            _connections.Remove(item);
        }
    }
}
