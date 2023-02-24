﻿using Akka.Actor;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Game;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Infrastructure.Statuses.Messages;

namespace OpenttdDiscord.Infrastructure.Statuses.Actors
{
    internal class StatusMonitorActor : ReceiveActorBase, IWithTimers
    {
        private readonly OttdServer ottdServer;

        private readonly StatusMonitor statusMonitor;

        private readonly AdminPortClient client;

        private readonly DiscordSocketClient discord;

        private static readonly ServerStatusEmbedBuilder embedBuilder = new();

        public ITimerScheduler Timers { get; set; } = default!;

        public StatusMonitorActor(
            OttdServer ottdServer,
            StatusMonitor statusMonitor,
            AdminPortClient client,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.ottdServer = ottdServer;
            this.client = client;
            this.statusMonitor = statusMonitor;
            this.discord = SP.GetRequiredService<DiscordSocketClient>();
            Ready();
            Timers.StartPeriodicTimer("regenerate", new RegenerateStatusMonitor(), TimeSpan.FromMinutes(1));
            Self.Tell(new RegenerateStatusMonitor());
        }

        private void Ready()
        {
            ReceiveAsync<RegenerateStatusMonitor>(RegenerateStatusMonitor);
        }

        public static Props Create(
            OttdServer server, 
            StatusMonitor monitor,
            AdminPortClient client,
            IServiceProvider serviceProvider) 
            => Props.Create(() => new StatusMonitorActor(server, monitor, client, serviceProvider));

        private async Task RegenerateStatusMonitor(RegenerateStatusMonitor _)
        {
            ServerStatus serverStatus = await client.QueryServerStatus();
            AdminServerInfo info = serverStatus.AdminServerInfo;

            Embed embed = embedBuilder.CreateServerStatusEmbed(client, serverStatus, info, ottdServer.Name);
            var channel = (IMessageChannel)await discord.GetChannelAsync(statusMonitor.ChannelId);
            var message = await channel.GetMessageAsync(statusMonitor.MessageId) as RestUserMessage;

            if (message == null)
            {
                return;
            }

            await message.ModifyAsync(x =>
            {
                x.Content = null;
                x.Embed = embed;
            });

            logger.LogDebug("Regenerated status monitor for {0} at {1}", ottdServer.Name, statusMonitor.ChannelId);
        }
    }
}
