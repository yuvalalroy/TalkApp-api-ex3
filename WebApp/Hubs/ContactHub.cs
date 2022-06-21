using WebApp.Models;
using Microsoft.AspNetCore.SignalR;
using WebApp.Services;

namespace WebApp.Hubs
{
    public class ContactHub : Hub
    {
        private readonly IDictionary<string, string> _connections;

        public ContactHub(IDictionary<string, string> connections)
        {
            _connections = connections;
        }

        public async Task AddContact(string userName, Contact contact)
        {
            try
            {
                var connectionId = _connections[userName];
                await Clients.Client(connectionId).SendAsync("ReceiveContact", new ContactPost
                {
                    id = contact.id,
                    last = contact.last,
                    lastdate = contact.lastdate,
                    name = contact.name,
                    server = contact.server
                }
            );
            }
            catch (Exception ex)
            {
                return;

            }

        }

        public async Task ContactUpdate(string userName, Contact contact)
        {
            try
            {
                var connectionId = _connections[userName];
                await Clients.Client(connectionId).SendAsync("ContactUpdate", contact.id);
            }
            catch (Exception ex)
            {
                return;

            }

            

        }

        public async Task ConnectClientToChat(string userConnection)
        {
            _connections[userConnection] = Context.ConnectionId;
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            var item = _connections.First(k => k.Value.Equals(Context.ConnectionId));
            _connections.Remove(item);
        }
    }
}
