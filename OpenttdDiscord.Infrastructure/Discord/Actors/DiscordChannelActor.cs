using Akka.Actor;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Infrastructure.Discord.Messages;

namespace OpenttdDiscord.Infrastructure.Discord.Actors
{
    internal class DiscordChannelActor : ReceiveActorBase
    {
        private readonly DiscordSocketClient discord;
        private readonly ulong channelId;
        private readonly IActorRef parent;
        public DiscordChannelActor(
            IServiceProvider serviceProvider, ulong channelId) : base(serviceProvider)
        {
            this.discord = serviceProvider.GetRequiredService<DiscordSocketClient>();
            this.channelId = channelId;
            this.parent = Context.Parent;

            Ready();
            Self.Tell(new InitDiscordChannelActor());
        }

        public static Props Create(IServiceProvider sp, ulong channelId)
            => Props.Create(() => new DiscordChannelActor(sp, channelId));

        private void Ready()
        {
            ReceiveAsync<InitDiscordChannelActor>(InitDiscordChannelActor);
        }

        private Task InitDiscordChannelActor(InitDiscordChannelActor _)
        {
            discord.MessageReceived += Discord_MessageReceived;
            return Task.CompletedTask;
        }

        private Task Discord_MessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage msg))
            {
                return Task.CompletedTask;
            }

            if(msg.Channel.Id != channelId)
            {
                return Task.CompletedTask;
            }

            if (string.IsNullOrWhiteSpace(msg.Content))
            {
                return Task.CompletedTask;
            }


            parent.Tell(new HandleDiscordMessage(msg.Author.Username, msg.Content));
            return Task.CompletedTask;
        }
    }
}
