using Discord;
using Discord.Rest;
using Discord.WebSocket;
using OpenttdDiscord.Backend.Servers;
using OpenttdDiscord.Messaging;
using OpenttdDiscord.Openttd.Network;
using OpenttdDiscord.Openttd.Network.Udp;
using System;
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

        private readonly List<SubscribedServer> servers = new List<SubscribedServer>();

        public ServerInfoProcessor(DiscordSocketClient client, ISubscribedServerService subscribedServerService, IOttdClientProvider ottdClientProvider, IEmbedFactory embedFactory)
        {
            this.subscribedServerService = subscribedServerService;
            this.client = client;
            this.ottdClientProvider = ottdClientProvider;
            this.embedFactory = embedFactory;

            this.subscribedServerService.ServerAdded += (_, ss) => servers.Add(ss);
        }

        public async Task Start()
        {
            servers.AddRange(await subscribedServerService.GetAllServers());

            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop()), null);
        }

        private async void MainLoop()
        {
            while (true)
            {
                try
                {
                    foreach (var s in servers)
                    {
                        var channel = client.GetChannel(s.ChannelId) as SocketTextChannel;
                        ulong? messageId = s.MessageId;
                        if (messageId.HasValue == false || (await channel.GetMessageAsync(messageId.Value)) == null)
                        {
                            messageId = (await channel.SendMessageAsync("Getting server info")).Id;
                        }
                        if (messageId.HasValue)
                        {
                            var ottdClient = this.ottdClientProvider.Provide(s.Server.ServerIp, s.Server.ServerPort);
                            var r = await ottdClient.AskAboutServerInfo();

                            Embed embed = embedFactory.Create(r, s.Server);


                            await (await channel.GetMessageAsync(messageId.Value) as RestUserMessage).ModifyAsync(x =>
                            {
                                x.Embed = embed;
                                x.Content = string.Empty;
                            });

                            await subscribedServerService.UpdateServer(s.Server.Id, s.ChannelId, messageId.Value);
                        }
                    }
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

    }
}
