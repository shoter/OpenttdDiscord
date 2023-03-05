using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Dispatch;
using LanguageExt.Pipes;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Chatting.Messages;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Chatting.Actors
{
    /// <summary>
    /// Used to store chat messages for given ottd server. It automatically connects to the ottd server event chain to gather chat messages.
    /// </summary>
    internal class ChatStorageActor : ReceiveActorBase, IWithTimers
    {
        // TODO: Make it configurable through appsettings or something or whatever
        private const int ChatMessageMaxCount = 600;

        private readonly OttdServer ottdServer;

        private readonly IAdminPortClient ottdClient;

        private readonly Queue<string> chatMessages = new();

        private DateTime lastMessageTime = DateTime.MaxValue;

        private DateTime lastMessageStoreTime = DateTime.MinValue;

        public ITimerScheduler Timers { get; set; } = default!;

        public ChatStorageActor(IServiceProvider serviceProvider, OttdServer server, IAdminPortClient ottdClient)
            : base(serviceProvider)
        {
            this.ottdServer = server;
            this.ottdClient = ottdClient;

            Ready();

            parent.Tell(new SubscribeToAdminEvents(Self));
            Timers.StartPeriodicTimer("store", new StoreChatMessages(), TimeSpan.FromMinutes(2));

            if (File.Exists(GetChatFileName()))
            {
                foreach (var line in File.ReadAllLines(GetChatFileName()))
                {
                    Enque(line);
                }
            }
        }

        public static Props Create(IServiceProvider serviceProvider, OttdServer server, IAdminPortClient ottdClient)
            => Props.Create(() => new ChatStorageActor(serviceProvider, server, ottdClient));

        private void Ready()
        {
            Receive<string>(HandleChatMessage);
            ReceiveAsync<AdminChatMessageEvent>(HandleChatMessage);
            ReceiveAsync<StoreChatMessages>(StoreChatMessages);
            Receive<RetrieveChatMessages>(RetrieveChatMessages);
            ReceiveIgnore<IAdminEvent>();
        }

        private void HandleChatMessage(string msg)
        {
            Enque(msg);

            lastMessageTime = DateTime.Now;
        }

        private void Enque(string msg)
        {
            chatMessages.Enqueue(msg);

            if (chatMessages.Count > ChatMessageMaxCount)
            {
                chatMessages.Dequeue();
            }
        }

        private async Task StoreChatMessages(StoreChatMessages _)
        {
            if (lastMessageStoreTime == lastMessageTime)
            {
                return;
            }

            StringBuilder sb = new();

            foreach (var msg in chatMessages)
            {
                sb.AppendLine(msg);
            }

            await File.WriteAllTextAsync(GetChatFileName(), sb.ToString());

            lastMessageStoreTime = lastMessageTime;
        }

        private async Task HandleChatMessage(AdminChatMessageEvent msg)
        {
            if (msg.NetworkAction != NetworkAction.NETWORK_ACTION_SERVER_MESSAGE)
            {
                return;
            }

            // This one should be quick awaitable
            ServerStatus info = await ottdClient.QueryServerStatus();
            uint clientId = msg.Player.ClientId;
            string playerIp = "no-ip";
            if (info.Players.ContainsKey(clientId))
            {
                playerIp = info.Players[clientId].Hostname;
            }

            string str = $"[{DateTime.Now:dd/MM HH:mm:ss}] {msg.Player.Name}({playerIp}): {msg.Message}";
            self.Tell(str);
        }

        private void RetrieveChatMessages(RetrieveChatMessages _)
        {
            List<string> messages = new List<string>(this.chatMessages.Count);
            messages.AddRange(this.chatMessages);
            Sender.Tell(new RetrievedChatMessages(messages));
        }

        protected override void PostStop()
        {
            base.PostStop();
            parent.Tell(new UnsubscribeFromAdminEvents(Self));
        }

        private string GetChatFileName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return $"./chat/{ottdServer.Id}.chat";
            }

            return $"/var/app/ottd/{ottdServer.Id}.chat";
        }
    }
}
