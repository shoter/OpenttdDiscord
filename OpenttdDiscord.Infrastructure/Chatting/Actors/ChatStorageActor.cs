using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using LanguageExt.Pipes;
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
        private const int ChatMessageMaxCount = 300;

        private readonly OttdServer ottdServer;

        private readonly Queue<string> chatMessages = new();

        private DateTime lastMessageTime = DateTime.MaxValue;

        private DateTime lastMessageStoreTime = DateTime.MinValue;

        private string ChatFileName => $"./chat/{ottdServer.Id}.chat";

        public ITimerScheduler Timers { get; set; } = default!;

        protected ChatStorageActor(IServiceProvider serviceProvider, OttdServer server)
            : base(serviceProvider)
        {
            this.ottdServer = server;

            Ready();

            parent.Tell(new SubscribeToAdminEvents(Self));
            Timers.StartPeriodicTimer("store", new StoreChatMessages(), TimeSpan.FromMinutes(10));

            if (File.Exists(ChatFileName))
            {
                foreach (var line in File.ReadAllLines(ChatFileName))
                {
                    Enque(line);
                }
            }
        }

        private void Ready()
        {
            Receive<string>(HandleChatMessage);
            Receive<AdminChatMessageEvent>(HandleChatMessage);
            ReceiveIgnore<IAdminEvent>();
            ReceiveAsync<StoreChatMessages>(StoreChatMessages);
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

            await File.WriteAllTextAsync(ChatFileName, sb.ToString());

            lastMessageStoreTime = lastMessageTime;
        }

        private void HandleChatMessage(AdminChatMessageEvent msg)
        {
            if (msg.NetworkAction != NetworkAction.NETWORK_ACTION_SERVER_MESSAGE)
            {
                return;
            }

            string str = $"[{DateTime.Now:HH:mm:ss}] {msg.Player.Name}: {msg.Message}";
            self.Tell(str);
        }

        protected override void PostStop()
        {
            base.PostStop();
            parent.Tell(new UnsubscribeFromAdminEvents(Self));
        }
    }
}
