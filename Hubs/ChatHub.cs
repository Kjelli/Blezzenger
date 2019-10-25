using Microsoft.AspNetCore.SignalR;

namespace Blezzenger.Hubs
{
    public class ChatHub : Hub
    {
        public static readonly string SendMessageMethod = "_sendMessage";
        public void SendMessage(string sender, string message)
        {
            Clients.Others.SendAsync(SendMessageMethod, sender, message);
        }
    }
}
