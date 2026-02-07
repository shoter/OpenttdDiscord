using System.Runtime.InteropServices;
using System.Text;
using Akka.Actor;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Chatting.Messages;
using OpenttdDiscord.Infrastructure.Discord.Messages;
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
        public const int ChatMessageMaxCount = 600;

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
            Receive<AdminClientJoinEvent>(HandleClientJoin);
            Receive<AdminClientDisconnectEvent>(HandleClientDisconnect);
            ReceiveAsync<StoreChatMessages>(StoreChatMessages);
            Receive<RetrieveEventLog>(RetrieveChatMessages);
            Receive<HandleDiscordMessage>(HandleDiscordMessage);
            Receive<HandleOttdMessage>(HandleOttdMessage);
            ReceiveAsync<AdminCompanyInfoEvent>(HandleNewCompany);
            ReceiveAsync<AdminCompanyUpdateEvent>(HandleCompanyUpdate);
            ReceiveAsync<AdminCompanyRemovalEvent>(HandleCompanyRemoval);
            ReceiveIgnore<IAdminEvent>();
        }

        private async Task HandleCompanyRemoval(AdminCompanyRemovalEvent msg)
        {
            var status = await ottdClient.QueryServerStatus();
            var players = string.Join(',',
                                      status.Players
                                          .Values
                                          .Where(p => p.PlayingAs == msg.Company.Id)
                                          .Select(p => $"{p.Name}({p.ClientId})[{p.Hostname}]"));
            string str = CreateMessage($"Company {msg.Company.Name}({msg.Company.Id + 1}) has been removed (owners: {players})");
            Self.Tell(str);
        }

        private async Task HandleNewCompany(AdminCompanyInfoEvent msg)
        {
            var status = await ottdClient.QueryServerStatus();
            var players = string.Join(',',
                                      status.Players
                                          .Values
                                          .Where(p => p.PlayingAs == msg.Company.Id)
                                          .Select(p => $"{p.Name}({p.ClientId})[{p.Hostname}]"));
            string str = CreateMessage($"Company {msg.Company.Name}({msg.Company.Id + 1}) has been created (owners: {players})");
            Self.Tell(str);
        }

        private async Task HandleCompanyUpdate(AdminCompanyUpdateEvent msg)
        {
            var status = await ottdClient.QueryServerStatus();
            var players = string.Join(',',
                                      status.Players
                                          .Values
                                          .Where(p => p.PlayingAs == msg.Company.Id)
                                          .Select(p => $"{p.Name}({p.ClientId})[{p.Hostname}]"));
            string str = CreateMessage($"Company {msg.Company.Name}({msg.Company.Id + 1}) has been updated (owners: {players})");
            Self.Tell(str);
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

            string filePath = GetChatFileName();
            string directoryPath = Path.GetDirectoryName(filePath)!;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            await File.WriteAllTextAsync(filePath, sb.ToString());

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

            string str = CreateMessage($"{msg.Player.Name}({playerIp}): {msg.Message}");
            self.Tell(str);
        }

        private static string CreateMessage(string message)
        {
            return $"[{DateTime.Now:dd/MM HH:mm:ss}] {message}";
        }

        private void HandleClientJoin(AdminClientJoinEvent ev)
        {
            string msg = CreateMessage($"{ev.Player.Name}({ev.Player.Hostname}) joins the game");
            self.Tell(msg);
        }

        private void HandleClientDisconnect(AdminClientDisconnectEvent ev)
        {
            string msg = CreateMessage($"{ev.Player.Name}({ev.Player.Hostname}) disconnected");
            self.Tell(msg);
        }

        private void HandleDiscordMessage(HandleDiscordMessage msg)
        {
            string str = $"[{DateTime.Now:dd/MM HH:mm:ss}] [Discord] {msg.Username}: {msg.Message}";
            self.Tell(str);
        }

        private void HandleOttdMessage(HandleOttdMessage msg)
        {
            string str = $"[{DateTime.Now:dd/MM HH:mm:ss}] [{msg.Server.Name}] {msg.Username}: {msg.Message}";
            self.Tell(str);
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