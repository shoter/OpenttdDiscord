using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Chatting.Messages;
using OpenttdDiscord.Infrastructure.Discord.Messages;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Chatting.Actors
{
    internal class DiscordCommunicationActor : ReceiveActorBase
    {
        private readonly ulong channelId;
        private readonly OttdServer ottdServer;
        private readonly AdminPortClient client;
        private readonly IAkkaService akkaService;
        private Option<IActorRef> chatChannel = Option<IActorRef>.None;

        public DiscordCommunicationActor(
            IServiceProvider serviceProvider,
            ulong channelId,
            AdminPortClient client,
            OttdServer ottdServer)
            : base(serviceProvider)
        {
            this.ottdServer = ottdServer;
            this.channelId = channelId;
            this.client = client;
            this.akkaService = SP.GetRequiredService<IAkkaService>();

            Ready();
            Self.Tell(new InitDiscordChannel());
        }

        public static Props Create(IServiceProvider serviceProvider, ulong channelId, AdminPortClient client, OttdServer ottdServer)
            => Props.Create(() => new DiscordCommunicationActor(serviceProvider, channelId, client, ottdServer));

        private void Ready()
        {
            ReceiveAsync<InitDiscordChannel>(InitDiscordChannel);
            Receive<HandleDiscordMessage>(HandleDiscordMessage);
            ReceiveIgnore<HandleOttdMessage>();
        }

        private async Task InitDiscordChannel(InitDiscordChannel _)
        {
            var msg = new GetCreateChatChannel(channelId);
            var self = Self;

            (await (
            from chatManager in akkaService.SelectActor(MainActors.Paths.ChatChannelManager)
            from channelActor in chatManager.TryAsk<IActorRef>(msg)
            from _1 in AssignChannelActor(channelActor).ToAsync()
            from _2 in channelActor.TryAsk(new RegisterToChatChannel(self))
            select _2
            )).ThrowIfError();
        }

        private void HandleDiscordMessage(HandleDiscordMessage handle)
        {
            var msg = new AdminChatMessage(
                NetworkAction.NETWORK_ACTION_CHAT,
                ChatDestination.DESTTYPE_BROADCAST,
                default,
                $"[Discord] {handle.Username}: {handle.Message}");
            client.SendMessage(msg);
        }

        private EitherUnit AssignChannelActor(IActorRef channelActor)
        {
            this.chatChannel = Option<IActorRef>.Some(channelActor);
            return Unit.Default;
        }

        protected override void PostStop()
        {
            base.PostStop();
            var self = Self;
            chatChannel.Some(a => a.Tell(new UnregisterFromChatChannel(self)));
        }
    }
}
