using System.Runtime.InteropServices;
using System.Text;
using Akka.Actor;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Chatting.Messages;
using OpenttdDiscord.Infrastructure.EventLogs.Messages;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.EventLogs.Actors
{
    /// <summary>
    /// Used to store chat messages for given ottd server. It automatically connects to the ottd server event chain to gather chat messages.
    /// </summary>
    internal class EventStorageActor : ReceiveActorBase, IWithTimers
    {
        // TODO: Make it configurable through appsettings or something or whatever
        private const int ChatMessageMaxCount = 600;

        private readonly OttdServer ottdServer;

        private readonly IAdminPortClient ottdClient;

        private readonly Queue<string> chatMessages = new();

        private DateTime lastMessageTime = DateTime.MaxValue;

        private DateTime lastMessageStoreTime = DateTime.MinValue;

        public ITimerScheduler Timers { get; set; } = default!;

        public EventStorageActor(IServiceProvider serviceProvider, OttdServer server, IAdminPortClient ottdClient)
            : base(serviceProvider)
        {
            ottdServer = server;
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
            => Props.Create(() => new EventStorageActor(serviceProvider, server, ottdClient));

        private void Ready()
        {
            Receive<string>(HandleChatMessage);
            ReceiveAsync<AdminChatMessageEvent>(HandleChatMessage);
            Receive<AdminConsoleEvent>(HandleConsole);
            ReceiveAsync<StoreChatMessages>(StoreChatMessages);
            Receive<RetrieveEventLog>(RetrieveChatMessages);
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

        private void HandleConsole(AdminConsoleEvent ev)
        {
            if (ev.EventType != AdminEventType.ConsoleMessage)
            {
                return;
            }

            if (ev.Message.Trim().StartsWith("[All]"))
            {
                // Do not print here messages written by players
                return;
            }

            string str = $"[{DateTime.Now:dd/MM HH:mm:ss}] {ev.Message}";
            Self.Tell(str);
        }

        private void RetrieveChatMessages(RetrieveEventLog _)
        {
            List<string> messages = new List<string>(chatMessages.Count);
            messages.AddRange(chatMessages);
            Sender.Tell(new RetrievedEventLog(messages));
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
