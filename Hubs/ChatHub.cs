using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blezzenger.Hubs
{
    public class ChatHub : Hub
    {
        public static readonly string MessageReceived = "_sendMessage";
        public static readonly string HelloReceived = "_sendHello";
        public static readonly string HelloBackReceived = "_sendHelloBack";
        public static readonly string GoodbyeReceived = "_goodbye";

        private Dictionary<string, string> _connections = new Dictionary<string, string>();

        public void SendMessage(Guid senderId, string senderName, string message)
        {
            Clients.Others.SendAsync(MessageReceived, senderId, senderName, message);
        }

        public void SendHello(string connectionId, Guid joinerId, string joinerName)
        {
            _connections.Add(connectionId, joinerName);
            Clients.AllExcept(connectionId).SendAsync(HelloReceived, connectionId, joinerId, joinerName);
        }

        public void SendHelloBack(string connectionId, Guid participantId, string participantName)
        {
            Clients.Client(connectionId).SendAsync(HelloBackReceived, participantId, participantName);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var disconnecterId = Context.ConnectionId;
            var name = _connections[disconnecterId];
            _connections.Remove(disconnecterId);
            SendMessage(Guid.Empty, "System", $"{name} has left the room");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
