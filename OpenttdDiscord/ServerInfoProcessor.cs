using Discord;
using Discord.Rest;
using Discord.WebSocket;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Messaging;
using OpenttdDiscord.Openttd.Network;
using OpenttdDiscord.Openttd.Network.Udp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenttdDiscord
{
    public class ServerInfoProcessor
    {
        private readonly ISubscribedServerService subscribedServerService;
        private readonly IOttdClientProvider ottdClientProvider;
        private readonly IEmbedFactory embedFactory;
        private readonly DiscordSocketClient client;

        private readonly ConcurrentDictionary<(ulong, ulong), SubscribedServer> servers = new ConcurrentDictionary<(ulong, ulong), SubscribedServer>();
        private readonly ConcurrentQueue<SubscribedServer> removedServers = new ConcurrentQueue<SubscribedServer>();

        public ServerInfoProcessor(DiscordSocketClient client, ISubscribedServerService subscribedServerService,
            IOttdClientProvider ottdClientProvider, IEmbedFactory embedFactory)
        {
            this.subscribedServerService = subscribedServerService;
            this.client = client;
            this.ottdClientProvider = ottdClientProvider;
            this.embedFactory = embedFactory;

            this.subscribedServerService.ServerAdded += (_, ss) => servers.TryAdd((ss.Server.Id, ss.ChannelId), ss);
            this.subscribedServerService.ServerRemoved += SubscribedServerService_ServerRemoved;
        }

        private void SubscribedServerService_ServerRemoved(object sender, SubscribedServer e)
        {
            this.removedServers.Enqueue(e);
        }

        public Task Start()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop()), null);
            return Task.CompletedTask;
        }

        private async void MainLoop()
        {
            foreach (var s in await subscribedServerService.GetAllServers())
                servers.TryAdd((s.Server.Id, s.ChannelId), s);

            while (true)
            {
                try
                {
                    await UpdateMessages();
                    await RemoveUnusedServers();
#if DEBUG
                    await Task.Delay(TimeSpan.FromSeconds(5));
#else
                    await Task.Delay(TimeSpan.FromMinutes(1));
#endif
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception " + ex.ToString());
                }
            }
        }

        private async Task RemoveUnusedServers()
        {
            while(this.removedServers.TryDequeue(out SubscribedServer s))
            {
                this.servers.TryRemove((s.Server.Id, s.ChannelId), out _);

                if (s.MessageId.HasValue == false)
                    continue;

                var channel = client.GetChannel(s.ChannelId) as SocketTextChannel;
                var message = await channel.GetMessageAsync(s.MessageId.Value) as RestUserMessage;

                await message.DeleteAsync();
            }
        }

        private async Task UpdateMessages()
        {
            foreach(var kp in servers)
            {
                var s = kp.Value;

                var channel = client.GetChannel(s.ChannelId) as SocketTextChannel;
                ulong? messageId = s.MessageId;
                if (messageId.HasValue == false || (await channel.GetMessageAsync(messageId.Value)) == null)
                {
                    messageId = (await channel.SendMessageAsync("Getting server info")).Id;
                }

                if (messageId.HasValue)
                {
                    var ottdClient = this.ottdClientProvider.Provide(s.Server.ServerIp, s.Port);
                    var r = await ottdClient.AskAboutServerInfo();

                    Embed embed = embedFactory.Create(r, s.Server);
                    var msg = await channel.GetMessageAsync(messageId.Value) as RestUserMessage;
                    await msg.ModifyAsync(x =>
                    {
                        x.Embed = embed;
                    });

                    await subscribedServerService.UpdateServer(s.Server.Id, s.ChannelId, messageId.Value);
                }
                var nv = new SubscribedServer(s.Server, s.LastUpdate, s.ChannelId, messageId, s.Port);
                servers.TryUpdate((s.Server.Id, s.ChannelId), nv, s); 
            }
        }
    }
}