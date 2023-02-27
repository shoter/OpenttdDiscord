using Akka.Actor;
using Discord;
using Discord.WebSocket;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Infrastructure.Chatting.Messages;
using OpenttdDiscord.Infrastructure.Discord.Messages;

namespace OpenttdDiscord.Infrastructure.Discord.Actors
{
    internal class DiscordChannelActor : ReceiveActorBase
    {
        private readonly DiscordSocketClient discord;
        private readonly ulong channelId;
        private Option<IMessageChannel> messageChannel = new();
        public DiscordChannelActor(
            IServiceProvider serviceProvider, ulong channelId) : base(serviceProvider)
        {
            this.discord = serviceProvider.GetRequiredService<DiscordSocketClient>();
            this.channelId = channelId;

            Ready();
            Self.Tell(new InitDiscordChannelActor());
        }

        public static Props Create(IServiceProvider sp, ulong channelId)
            => Props.Create(() => new DiscordChannelActor(sp, channelId));

        private void Ready()
        {
            ReceiveAsync<InitDiscordChannelActor>(InitDiscordChannelActor);
            ReceiveAsync<HandleOttdMessage>(HandleOttdMessage);
            ReceiveIgnore<HandleDiscordMessage>();
        }

        private async Task InitDiscordChannelActor(InitDiscordChannelActor _)
        {
            discord.MessageReceived += Discord_MessageReceived;
            messageChannel = Some((IMessageChannel)await discord.GetChannelAsync(channelId));
            parent.Tell(new RegisterToChatChannel(self));
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

            if(discord.CurrentUser.Id == msg.Author.Id)
            {
                return Task.CompletedTask;
            }

            parent.Tell(new HandleDiscordMessage(msg.Author.Username, msg.Content));
            return Task.CompletedTask;
        }

        private async Task HandleOttdMessage(HandleOttdMessage msg)
        {
            string message = $"[{msg.Server.Name} {msg.Username}: {msg.Message}";
            await messageChannel.IfSomeAsync(async channel =>
            {
                await channel.SendMessageAsync(message);
            });
        }

        protected override void PostStop()
        {
            base.PostStop();
            parent.Tell(new UnregisterFromChatChannel(self));
        }
    }
}
