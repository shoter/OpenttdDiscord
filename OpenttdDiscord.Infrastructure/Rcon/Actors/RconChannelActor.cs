using System.Text;
using Akka.Actor;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Ottd.Messages;
using OpenttdDiscord.Infrastructure.Rcon.Messages;

namespace OpenttdDiscord.Infrastructure.Rcon.Actors
{
    internal class RconChannelActor : ReceiveActorBase, IWithTimers
    {
        private readonly RconChannel channel;

        private readonly OttdServer server;

        private readonly IAdminPortClient client;

        private readonly IMessageChannel messageChannel;

        private readonly DiscordSocketClient discord;

        private readonly StringBuilder messageToBeSent = new();

        public ITimerScheduler Timers { get; set; } = default!;

        public RconChannelActor(
            IServiceProvider sp,
            RconChannel channel,
            OttdServer server,
            IAdminPortClient client)
            : base(sp)
        {
            this.channel = channel;
            this.server = server;
            this.client = client;
            this.discord = SP.GetRequiredService<DiscordSocketClient>();
            this.messageChannel = (IMessageChannel)discord.GetChannel(channel.ChannelId);

            Ready();

            discord.MessageReceived += Discord_MessageReceived;
            parent.Tell(new SubscribeToAdminEvents(Self));
            logger.LogInformation($"Rcon channel {this} created");
        }

        private Task Discord_MessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage msg))
            {
                return Task.CompletedTask;
            }

            if (msg.Channel.Id != channel.ChannelId)
            {
                return Task.CompletedTask;
            }

            if (string.IsNullOrWhiteSpace(msg.Content))
            {
                return Task.CompletedTask;
            }

            if(!msg.Content.StartsWith(channel.Prefix))
            {
                return Task.CompletedTask;
            }

            if (discord.CurrentUser.Id == msg.Author.Id)
            {
                return Task.CompletedTask;
            }

            string command = msg.Content.Split(channel.Prefix).Last();
            self.Tell(new HandleRconMessage(command));
            return Task.CompletedTask;
        }

        public static Props Create(IServiceProvider sp, RconChannel channel, OttdServer server, IAdminPortClient client)
            => Props.Create(() => new RconChannelActor(sp, channel, server, client));

        private void Ready()
        {
            Receive<HandleRconMessage>(HandleRconMessage);
            ReceiveAsync<SentQueuedRconText>(SentQueuedRconText);
            Receive<AdminRconEvent>(HandleRconEvent);
            ReceiveIgnore<IAdminEvent>();
        }

        private void HandleRconMessage(HandleRconMessage msg)
        {
            client.SendMessage(new AdminRconMessage(msg.Command));
            Timers.StartSingleTimer("SentQueuedRconText", new SentQueuedRconText(), TimeSpan.FromSeconds(1));
        }

        private void HandleRconEvent(AdminRconEvent e)
        {
            messageToBeSent.AppendLine(e.Message);
        }

        private async Task SentQueuedRconText(SentQueuedRconText _)
        {
            if(messageToBeSent.Length == 0)
            {
                return;
            }

            string message = messageToBeSent.ToString();
            await messageChannel.SendMessageAsync(message);
            messageToBeSent.Clear();
        }

        protected override void PostStop()
        {
            base.PostStop();
            parent.Tell(new UnsubscribeFromAdminEvents(Self));
            logger.LogInformation($"Rcon channel {this} stopped");
        }

        public override string ToString() => $"{server.Id} - {channel.ChannelId}";
    }
}
