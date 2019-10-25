using Blezzenger.Db;
using Blezzenger.Hubs;
using Blezzenger.Models;
using Blezzenger.Services;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blezzenger.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;


        private readonly SessionStorage _session;
        private readonly ChatContext _context;
        private HubConnection _connection;
        private bool _isSending;

        public string MessageInput { get; set; }
        public string UsernameInput { get; set; }

        public List<Message> Messages { get; }
        public bool? IsAuthenticated { get; set; }
        public bool HasLoadedMessages { get; set; }

        public ChatViewModel(IAuthenticationService authenticationService, SessionStorage session, ChatContext context)
        {
            _auth = authenticationService;
            _session = session;
            _context = context;

            Messages = new List<Message>();
        }


        public async override void OnInitialized()
        {

        }

        private void ConfigureConnection()
        {
            if (_connection == null)
            {
                _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/chatHub")
                .Build();
            }

            _connection.Closed += (error) =>
            {
                Messages.Add(BuildSystemMessage("Connection closed"));

                return Task.CompletedTask;
            };

            _connection.On<Guid, string, string>(ChatHub.MessageReceived, (senderId, senderName, message) =>
            {
                var isConsecutive = Messages.LastOrDefault()?.SenderId.Equals(senderId) == true;
                Messages.Add(BuildMessage(senderId, senderName, message));

                NotifyStateChange();
            });

            _connection.On<string, Guid, string>(ChatHub.HelloReceived, async (connectionId, joinerId, joinerName) =>
            {
                Messages.Add(BuildSystemMessage($"{joinerName} has entered the room"));
                await _connection.InvokeAsync(nameof(ChatHub.SendHelloBack), connectionId, _auth.CurrentUser.Id, _auth.CurrentUser.Username);

                NotifyStateChange();
            });

            _connection.On<Guid, string>(ChatHub.HelloBackReceived, (participantId, participant) =>
            {
                if (participantId == _auth.CurrentUser.Id)
                {
                    return;
                }

                Messages.Add(BuildSystemMessage($"{participant} is also here"));

                NotifyStateChange();
            });
        }

        public async void SelectedUsername()
        {
            var existing = _context.Users.FirstOrDefault(u => u.Username.Equals(UsernameInput));
            if (existing is null)
            {
                existing = new User { Id = Guid.NewGuid(), Username = UsernameInput };

                _context.Users.Add(existing);
                await _context.SaveChangesAsync();
            }

            await _session.SetItemAsync("auth", existing);
            _auth.CurrentUser = existing;
            IsAuthenticated = true;
        }


        public async void SendMessage()
        {
            if (_isSending) return;
            _isSending = true;

            var message = BuildMessage(_auth.CurrentUser.Id, _auth.CurrentUser.Username, MessageInput);
            await _connection.InvokeAsync(nameof(ChatHub.SendMessage), message.SenderId, message.SenderName, message.Content);

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            Messages.Add(message);

            _isSending = false;

            NotifyStateChange();
        }

        private Message BuildSystemMessage(string content)
        {
            return Decorate(new Message(Guid.Empty, "System", content));
        }

        private Message BuildMessage(Guid senderId, string senderName, string content)
        {
            var message = new Message(senderId, senderName, content);

            return Decorate(message);
        }

        private Message Decorate(Message message)
        {
            message.IsConsecutive = Messages.LastOrDefault()?.SenderId.Equals(message.SenderId) == true;
            message.IsSystemMessage = message.SenderId == Guid.Empty;
            message.IsSentByMe = message.SenderId == _auth.CurrentUser?.Id;

            return message;
        }

        private void LoadMessages(List<Message> messages)
        {
            Messages.AddRange(messages.Select(Decorate));
            HasLoadedMessages = true;

            NotifyStateChange();
        }

        public override async void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;

            CheckAuthentication();


            ConfigureConnection();

            if (_connection.State != HubConnectionState.Connected)
            {
                await _connection.StartAsync();
                await _connection.InvokeAsync(nameof(ChatHub.SendHello), _connection.ConnectionId, _auth.CurrentUser.Id, _auth.CurrentUser.Username);
            }
            LoadMessages(_context.Messages.ToList());
        }

        private async void CheckAuthentication()
        {
            if (await _session.GetItemAsync<User>("auth") is User user)
            {
                _auth.CurrentUser = user;
                IsAuthenticated = true;
            }
            else
            {
                IsAuthenticated = false;
                NotifyStateChange();
                return;
            }
        }
    }
}
