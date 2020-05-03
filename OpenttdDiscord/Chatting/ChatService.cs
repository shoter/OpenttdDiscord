using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Openttd;
using OpenttdDiscord.Openttd.Network;
using OpenttdDiscord.Openttd.Network.AdminPort;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenttdDiscord.Chatting
{
    public class ChatService : IChatService
    {
        private readonly ILogger<ChatService> logger;
        private readonly IChatChannelServerService chatChannelServerService;
        private readonly IAdminPortClientProvider adminPortClientProvider;
        private readonly DiscordSocketClient discord;

        private readonly ConcurrentDictionary<(ulong, ulong), ChatChannelServer> chatServers = new ConcurrentDictionary<(ulong, ulong), ChatChannelServer>();
        private readonly ConcurrentQueue<IAdminEvent> receivedMessagges = new ConcurrentQueue<IAdminEvent>();
        private readonly ConcurrentQueue<DiscordMessage> discordMessages = new ConcurrentQueue<DiscordMessage>();
        private readonly ConcurrentQueue<ChatChannelServer> serversToRemove = new ConcurrentQueue<ChatChannelServer>();

        public ChatService(ILogger<ChatService> logger, IChatChannelServerService chatChannelServerService, IAdminPortClientProvider adminPortClientProvider, DiscordSocketClient discord)
        {
            this.logger = logger;
            this.chatChannelServerService = chatChannelServerService;
            this.adminPortClientProvider = adminPortClientProvider;
            this.adminPortClientProvider.NewClientCreated += (_, client) => client.EventReceived += Client_EventReceived;
            this.discord = discord;
        }

        public async Task Start()
        {
            foreach (var s in await this.chatChannelServerService.GetAll())
                this.chatServers.TryAdd((s.Server.Id, s.ChannelId), s);

            this.chatChannelServerService.Added += (_, s) => chatServers.AddOrUpdate((s.Server.Id, s.ChannelId), s, (_,__) => s);
            this.chatChannelServerService.Removed += (_, s) => serversToRemove.Enqueue(s);

            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop()), null);
        }

        public async void MainLoop()
        {
            while (true)
            {
                try
                {
                    while(serversToRemove.TryDequeue(out ChatChannelServer s))
                    {
                        IAdminPortClient client = await adminPortClientProvider.GetClient(new ServerInfo(s.Server.ServerIp, s.Server.ServerPort, s.Server.ServerPassword));
                        chatServers.Remove((s.Server.Id, s.ChannelId), out _);
                    }

                    foreach (var s in chatServers.Values)
                    {
                        Server server = s.Server;
                        ServerInfo info = new ServerInfo(server.ServerIp, server.ServerPort, server.ServerPassword);
                        IAdminPortClient client = await adminPortClientProvider.GetClient(info);
                        if (client.ConnectionState == AdminConnectionState.Idle)
                        {
                            await client.Join();
                        }
                    }

                    while (receivedMessagges.TryDequeue(out IAdminEvent ev))
                    {
                        // for right now it is only event - this needs to be changed later
                        var msg = ev as AdminChatMessageEvent;

                        if (msg == null)
                            continue;

                        if (msg.Player.ClientId == 1)
                            continue;
                        Server s = chatServers.Values.FirstOrDefault(c => c.Server.ServerIp == msg.Server.ServerIp && c.Server.ServerPort == msg.Server.ServerPort)?.Server;

                        if (s == null)
                            continue;

                        IEnumerable<ChatChannelServer> csList = chatServers.Values.Where(x => x.Server.Id == s.Id);

                        foreach (var cs in csList)
                        {
                            var channel = discord.GetChannel(cs.ChannelId) as SocketTextChannel;

                            var chatMsg = $"[{cs.Server.ServerName}] {msg.Player.Name}: {msg.Message}";

                            await channel.SendMessageAsync(chatMsg);

                            IEnumerable<ChatChannelServer> others = chatServers.Values.Where(x => x.ChannelId == channel.Id && x.Server.Id != s.Id);
                            foreach (var o in others)
                            {
                                Server server = chatServers[(o.Server.Id, o.ChannelId)].Server;
                                var info = new ServerInfo(server.ServerIp, server.ServerPort, server.ServerPassword);
                                IAdminPortClient client = await adminPortClientProvider.GetClient(info);

                                if(client.ConnectionState == AdminConnectionState.Connected)
                                {
                                    client.SendMessage(new AdminChatMessage(NetworkAction.NETWORK_ACTION_CHAT, ChatDestination.DESTTYPE_BROADCAST, 0, chatMsg));
                                }
                            }
                        }
                    }

                    while(discordMessages.TryDequeue(out DiscordMessage msg))
                    {
                        var chatMsg = $"[Discord] {msg.Username}: {msg.Message}";

                        IEnumerable<ChatChannelServer> others = chatServers.Values.Where(x => x.ChannelId == msg.ChannelId);
                        foreach (var o in others)
                        {
                            Server server = chatServers[(o.Server.Id, o.ChannelId)].Server;
                            var info = new ServerInfo(server.ServerIp, server.ServerPort, server.ServerPassword);
                            IAdminPortClient client = await adminPortClientProvider.GetClient(info);

                            if (client.ConnectionState == AdminConnectionState.Connected)
                            {
                                client.SendMessage(new AdminChatMessage(NetworkAction.NETWORK_ACTION_CHAT, ChatDestination.DESTTYPE_BROADCAST, 0, chatMsg));
                            }
                        }


                    }

#if DEBUG
                    await Task.Delay(TimeSpan.FromSeconds(0.05));
#else
                    await Task.Delay(TimeSpan.FromSeconds(0.3));
#endif
                }
                catch (Exception e)
                {
                    logger.LogError($"Error {e.Message}", e);
                }
            }
        }

        private void Client_EventReceived(object sender, IAdminEvent msg)
        {
            this.receivedMessagges.Enqueue(msg);
        }

        public void AddMessage(DiscordMessage message)
        {
            this.discordMessages.Enqueue(message);
        }
    }
}
