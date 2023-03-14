using Akka.Actor;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Game;
using OpenttdDiscord.Database.Statuses;
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

        private static readonly ServerStatusEmbedBuilder EmbedBuilder = new();

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

            logger.LogDebug($"MonitorActor created for {statusMonitor.ServerId}-{statusMonitor.ChannelId}");
        }

        private void Ready()
        {
            ReceiveAsync<RegenerateStatusMonitor>(RegenerateStatusMonitor);
            ReceiveAsync<RemoveStatusMonitor>(RemoveStatusMonitor);
        }

        public static Props Create(
            OttdServer server,
            StatusMonitor monitor,
            AdminPortClient client,
            IServiceProvider serviceProvider)
            => Props.Create(() => new StatusMonitorActor(server, monitor, client, serviceProvider));

        private async Task RegenerateStatusMonitor(RegenerateStatusMonitor _)
        {
            Embed embed = await CreateEmbed();
            Optional<RestUserMessage> message = await GetMessage();

            if (!message.IsSpecified)
            {
                await RegenerateMessage(embed);
                return;
            }

            await UpdateMessage(embed, message);
        }

        private async Task<Embed> CreateEmbed()
        {
            ServerStatus serverStatus = await client.QueryServerStatus();
            AdminServerInfo info = serverStatus.AdminServerInfo;

            return EmbedBuilder.CreateServerStatusEmbed(client, serverStatus, info, ottdServer.Name);
        }

        private async Task UpdateMessage(Embed embed, Optional<RestUserMessage> message)
        {
            await message.Value.ModifyAsync(x =>
            {
                x.Content = null;
                x.Embed = embed;
            });

            logger.LogDebug("Regenerated status monitor for {0} at {1}", ottdServer.Name, statusMonitor.ChannelId);
        }

        private async Task RegenerateMessage(Embed embed)
        {
            var channel = (IMessageChannel)await discord.GetChannelAsync(statusMonitor.ChannelId);
            var newMessage = await channel.SendMessageAsync(embed: embed);
            Context.Parent.Tell(new UpdateStatusMonitor(this.statusMonitor with
            {
                MessageId = newMessage.Id
            }));

            logger.LogDebug("No message detected. Created new - recreating status monitor for {0} at {1}", ottdServer.Name, statusMonitor.ChannelId);
        }

        private async Task<Optional<RestUserMessage>> GetMessage()
        {
            var channel = (IMessageChannel)await discord.GetChannelAsync(statusMonitor.ChannelId);
            var message = await channel.GetMessageAsync(statusMonitor.MessageId) as RestUserMessage;

            if (message == null)
            {
                return default;
            }

            return message;
        }

        private async Task RemoveStatusMonitor(RemoveStatusMonitor msg)
        {
            var message = await GetMessage();

            if (!message.IsSpecified)
            {
                return;
            }

            await message.Value.DeleteAsync();
        }

        public override void AroundPreRestart(Exception cause, object message)
        {
            logger.LogError($"{statusMonitor.ServerId}-{statusMonitor.ChannelId} is being restarted after {message} because:\n {cause}");
            base.AroundPreRestart(cause, message);
        }
    }
}
