using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Backend.Chatting;
using OpenttdDiscord.Backend.Servers;
using OpenttdDiscord.Openttd.Network;
using OpenttdDiscord.Openttd.Network.Tcp;
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
        private readonly IOttdClientProvider ottdClientProvider;
        private readonly IServerService serverService;
        private readonly DiscordSocketClient discord;

        private readonly ConcurrentDictionary<ulong, Server> servers = new ConcurrentDictionary<ulong, Server>();
        private readonly ConcurrentDictionary<ulong, ChatChannelServer> chatServers = new ConcurrentDictionary<ulong, ChatChannelServer>();
        private readonly ConcurrentQueue<ReceivedChatMessage> receivedMessagges = new ConcurrentQueue<ReceivedChatMessage>();
        private readonly ConcurrentQueue<DiscordMessage> discordMessages = new ConcurrentQueue<DiscordMessage>();

        public ChatService(ILogger<ChatService> logger, IChatChannelServerService chatChannelServerService, IOttdClientProvider ottdClientProvider, IServerService serverService,
            DiscordSocketClient discord)
        {
            this.logger = logger;
            this.chatChannelServerService = chatChannelServerService;
            this.ottdClientProvider = ottdClientProvider;
            this.serverService = serverService;
            this.discord = discord;
        }

        public async Task Start()
        {
            foreach (var s in await this.chatChannelServerService.GetAll())
                this.chatServers.TryAdd(s.ServerId, s);

            foreach (var s in await this.serverService.GetAll())
                this.servers.TryAdd(s.Id, s);

            this.chatChannelServerService.Added += (_, s) => chatServers.AddOrUpdate(s.ServerId, s, (_,__) => s);
            this.serverService.Added += (_, s) => servers.AddOrUpdate(s.Id, s, (_, __) => s);

            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop()), null);
        }

        private void AddServer(ChatChannelServer s)
        {
            this.chatServers.AddOrUpdate(s.ServerId, s, (_, __) => s);
        }

        private void Client_ReceivedChatMessage(object sender, ReceivedChatMessage e)
        {
            this.receivedMessagges.Enqueue(e);
        }

        public async void MainLoop()
        {
            while (true)
            {
                try
                {
                    foreach (var s in chatServers.Values)
                    {
                        Server server = servers[s.ServerId];

                        IOttdClient client = ottdClientProvider.Provide(server.ServerIp, server.ServerPort);

                        if (client.ConnectionState == ConnectionState.Idle)
                        {
                            await client.JoinGame("OpenTTDBot", "");
                            client.ReceivedChatMessage -= Client_ReceivedChatMessage;
                            client.ReceivedChatMessage += Client_ReceivedChatMessage;
                        }
                    }

                    while (receivedMessagges.TryDequeue(out ReceivedChatMessage msg))
                    {
                        if (msg.Player.ClientId == 1)
                            continue;
                        Server s = servers.Values.First(s => s.ServerIp == msg.ServerInfo.ServerIp && s.ServerPort == msg.ServerInfo.ServerPort);
                        IEnumerable<ChatChannelServer> csList = chatServers.Values.Where(x => x.ServerId == s.Id);

                        foreach (var cs in csList)
                        {
                            var channel = discord.GetChannel(cs.ChannelId) as SocketTextChannel;

                            var chatMsg = $"[{cs.ServerName}] {msg.Player.Name} : {msg.Message}";
                            await channel.SendMessageAsync(chatMsg);

                            IEnumerable<ChatChannelServer> others = chatServers.Values.Where(x => x.ChannelId == channel.Id && x.ServerId != s.Id);
                            foreach (var o in others)
                            {
                                Server server = servers[o.ServerId];
                                IOttdClient client = ottdClientProvider.Provide(server.ServerIp, server.ServerPort);

                                await client.SendChatMessage(chatMsg);
                            }
                        }
                    }

                    while(discordMessages.TryDequeue(out DiscordMessage msg))
                    {
                        var chatMsg = $"[Discord] {msg.Username} : {msg.Message}";

                        IEnumerable<ChatChannelServer> others = chatServers.Values.Where(x => x.ChannelId == msg.ChannelId);
                        foreach (var o in others)
                        {
                            Server server = servers[o.ServerId];
                            IOttdClient client = ottdClientProvider.Provide(server.ServerIp, server.ServerPort);

                            await client.SendChatMessage(chatMsg);
                        }


                    }
                  

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                catch (Exception e)
                {
                    logger.LogError("Error", e);
                }
            }
        }

        public void AddMessage(DiscordMessage message)
        {
            this.discordMessages.Enqueue(message);
        }
    }
}
