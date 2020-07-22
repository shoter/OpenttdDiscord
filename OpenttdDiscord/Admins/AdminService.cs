using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Backend.Admins;
using OpenttdDiscord.Database.Admins;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Openttd;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenttdDiscord.Admins
{
    public class AdminService : IAdminService, IAdminPortClientUser
    {
        private ConcurrentDictionary<(string ip, int port), ConcurrentQueue<IAdminEvent>> EventsQueue { get; } = new ConcurrentDictionary<(string ip, int port), ConcurrentQueue<IAdminEvent>>();
        private ConcurrentQueue<AdminChannel> AdminChannelsToRemove { get; } = new ConcurrentQueue<AdminChannel>();

        private ConcurrentDictionary<AdminChannelUniqueValue, AdminChannel> adminChannels = new ConcurrentDictionary<AdminChannelUniqueValue, AdminChannel>();
        private ConcurrentQueue<(ulong ChannelId, string Message)> messagesEnqued = new ConcurrentQueue<(ulong ChannelId, string Message)>();
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
            adminChannelService.Added += (_, ac) => adminChannels.TryAdd(ac.UniqueValue, ac);
            adminChannelService.Updated += AdminChannelService_Updated;
            adminChannelService.Removed += (_, ac) => AdminChannelsToRemove.Enqueue(ac);
        }

        private void AdminChannelService_Updated(object sender, AdminChannel e)
        {
            var old = adminChannels[e.UniqueValue];
            adminChannels.TryUpdate(e.UniqueValue, e, old);
        }

        public async Task Start()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop()), null);
            var adminChannels = await adminChannelService.GetAll();
            foreach (var ac in adminChannels)
                this.adminChannels.TryAdd(ac.UniqueValue, ac);
        }

        private async void MainLoop()
        {
            while (true)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));

                    await JoinUnjoinedClients();
                    await ProcessEvents();
                    await ProcessDiscordMessages();
                    await ProcessRemovedServers();
                }
                catch (Exception e)
                {
                    logger.LogInformation($"Admin Service - {e.Message}", e);
                }

            }
        }

        private async Task ProcessRemovedServers()
        {
            while (AdminChannelsToRemove.TryDequeue(out var adminChannel))
            {
                await clientProvider.Unregister(this, adminChannel.Server);
                adminChannels.TryRemove(adminChannel.UniqueValue, out _);
            }
        }

        private Task ProcessDiscordMessages()
        {
            while (messagesEnqued.TryDequeue(out var msg))
            {
                var adminChannel = adminChannels.Values.FirstOrDefault(ac => ac.ChannelId == msg.ChannelId);

                // we do not handle this admin channel anymore - it was removed.
                if (adminChannel == null)
                    continue;

                if (!msg.Message.StartsWith(adminChannel.Prefix))
                    continue;

                string command = msg.Message.Split(adminChannel.Prefix).Last();

                var client = clientProvider.GetClient(this, adminChannel.Server);
                client.SendMessage(new AdminRconMessage(command));
            }
            return Task.CompletedTask;

        }

        private async Task ProcessEvents()
        {
            foreach (var key in EventsQueue.Keys)
                if (EventsQueue.TryRemove(key, out var events))
                {
                    StringBuilder sb = new StringBuilder();
                    string msg = null;

                    while (events.TryDequeue(out var e))
                    {
                        if (e.EventType == AdminEventType.AdminRcon)
                        {
                            var rcon = e as AdminRconEvent;
                            sb.Append($"{rcon.Message}\n");
                        }
                    }

                    msg = sb.ToString();
                    if (string.IsNullOrWhiteSpace(msg))
                        continue;

                    var servers = adminChannels.Values.Where(a => a.Server.ServerIp == key.ip && a.Server.ServerPort == key.port).ToList();
                    foreach (var s in servers)
                    {
                        var channel = discord.GetChannel(s.ChannelId) as SocketTextChannel;
                        await channel.SendMessageAsync(msg);
                    }

                }
        }

        private async Task JoinUnjoinedClients()
        {
            foreach (var adminChannel in adminChannels.Values)
            {
                try
                {
                    if (clientProvider.IsRegistered(this, adminChannel.Server) == false)
                    {
                        await clientProvider.Register(this, adminChannel.Server);
                    }
                }
                catch (Exception e)
                {
                    logger.LogError($"{adminChannel.Server.ServerIp}:{adminChannel.Server.ServerPort} - cannot register - {e.Message}");
                }
            }
        }

        public Task HandleMessage(ulong channelId, string message)
        {
            messagesEnqued.Enqueue((channelId, message));
            return Task.CompletedTask;
        }

        public void ParseServerEvent(Server server, IAdminEvent adminEvent)
        {
            var queue = EventsQueue.GetOrAdd((server.ServerIp, server.ServerPort), (_) => new ConcurrentQueue<IAdminEvent>());
            queue.Enqueue(adminEvent);
        }
    }
}
