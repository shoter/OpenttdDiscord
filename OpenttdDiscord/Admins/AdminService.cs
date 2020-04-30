using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Database.Admins;
using OpenttdDiscord.Openttd;
using OpenttdDiscord.Openttd.Network.AdminPort;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenttdDiscord.Admins
{
    public class AdminService : IAdminService
    {
        private ConcurrentQueue<IAdminEvent> EventsQueue { get; } = new ConcurrentQueue<IAdminEvent>();
        private ConcurrentDictionary<AdminChannelUniqueValue, AdminChannel> adminChannels = new ConcurrentDictionary<AdminChannelUniqueValue, AdminChannel>();
        private ConcurrentQueue<(ulong ChannelId, string Message)> CommandsToExecute = new ConcurrentQueue<(ulong ChannelId, string Message)>();
        private readonly IAdminPortClientProvider clientProvider;
        private readonly DiscordSocketClient discord;
        private readonly IAdminChannelService adminChannelService;
        private readonly ILogger<IAdminService> logger;

        public AdminService(DiscordSocketClient discord, IAdminPortClientProvider clientProvider, IAdminChannelService adminChannelService, ILogger<IAdminService> logger)
        {
            this.clientProvider = clientProvider;
            this.discord = discord;
            this.adminChannelService = adminChannelService;
            this.logger = logger;
            clientProvider.NewClientCreated += ClientProvider_NewClientCreated;
            adminChannelService.Added += (_, ac) => adminChannels.TryAdd(ac.UniqueValue, ac);
        }

        private void ClientProvider_NewClientCreated(object sender, IAdminPortClient e) => e.EventReceived += (_, ev) => EventsQueue.Enqueue(ev);

        public Task Start()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop()), null);
            return Task.CompletedTask;
        }

        private async void MainLoop()
        {
            while (true)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                    while (EventsQueue.TryDequeue(out IAdminEvent e))
                    {
                        if (e.EventType == AdminEventType.ConsoleMessage)
                        {
                            var consoleEvent = e as AdminConsoleEvent;
                            var servers = adminChannels.Values.Where(a => a.Server.ServerIp == e.Server.ServerIp && a.Server.ServerPort == e.Server.ServerPort).ToList();


                            foreach (var s in servers)
                            {
                                var channel = discord.GetChannel(s.ChannelId) as SocketTextChannel;
                                await channel.SendMessageAsync($"{consoleEvent.Origin} - {consoleEvent.Message}");
                            }
                        }
                    }

                    while (CommandsToExecute.TryDequeue(out var cmd))
                    {
                        var adminChannels = await adminChannelService.GetAllForChannel(cmd.ChannelId);

                        foreach (var ac in adminChannels)
                        {
                            var client = await clientProvider.GetClient(new ServerInfo(ac.Server.ServerIp, ac.Server.ServerPort, ac.Server.ServerPassword));

                            if (client.ConnectionState != AdminConnectionState.Connected)
                            {
                                await client.Join();
                            }

                            client.SendMessage(new AdminRconMessage(cmd.Message));

                        }

                    }
                }
                catch(Exception e)
                {
                    logger.LogInformation($"Admin Service - {e.Message}", e);
                }

            }
        }

        public Task ExecuteCommand(ulong channelId, string command)
        {
            CommandsToExecute.Enqueue((channelId, command));
            return Task.CompletedTask;
        }
    }
}
