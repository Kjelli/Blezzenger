using Microsoft.AspNetCore.SignalR;

namespace Blezzenger.Hubs
{
    public class ChatHub : Hub
    {
        public static readonly string SendMessageMethod = "_sendMessage";
        public static readonly string SendHelloMethod = "_sendHello";
        public static readonly string SendHelloBackMethod = "_sendHelloBack";
        public void SendMessage(string sender, string message)
        {
            Clients.Others.SendAsync(SendMessageMethod, sender, message);
        }

        public void SendHello(string joiner)
        {
            Clients.Others.SendAsync(SendHelloMethod, joiner);
        }

        public void SendHelloBack(string me)
        {
            Clients.Others.SendAsync(SendHelloBackMethod, me);
        }
    }
}
