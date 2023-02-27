using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort.Events;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Chatting.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Infrastructure.Chatting.Actors
{
    internal class OttdCommunicationActor : ReceiveActorBase
    {
        private readonly OttdServer server;
        private readonly ulong channelId;
        private Option<IActorRef> chatChannel = Option<IActorRef>.None;

        private readonly IAkkaService akkaService;

        protected OttdCommunicationActor(
            IServiceProvider serviceProvider,
            ulong channelId,
            OttdServer server)
            : base(serviceProvider)
        {
            this.server = server;
            this.channelId = channelId;
            this.akkaService = SP.GetRequiredService<IAkkaService>();

            Ready();
            Self.Tell(new InitOttdCommunicationActor());
        }

        public static Props Create(IServiceProvider sp, ulong channelId, OttdServer server) =>
            Props.Create(() => new OttdCommunicationActor(sp, channelId, server));

        private void Ready()
        {
            ReceiveAsync<InitOttdCommunicationActor>(InitOttdCommunicationActor);
            Receive<AdminChatMessageEvent>(HandleChatMessage);
            // ignore other events
            Receive<IAdminEvent>(_ => { });
        }

        private async Task InitOttdCommunicationActor(InitOttdCommunicationActor _)
        {

            var msg = new GetCreateChatChannel(channelId);
            var self = Self;

            (await(
            from chatManager in akkaService.SelectActor(MainActors.Paths.ChatChannelManager)
            from channelActor in chatManager.TryAsk<IActorRef>(msg)
            from _1 in AssignChannelActor(channelActor).ToAsync()
            from _2 in channelActor.TryAsk(new RegisterToChatChannel(self))
            select _2
            )).ThrowIfError();
        }

        private EitherUnit AssignChannelActor(IActorRef channelActor)
        {
            this.chatChannel = Option<IActorRef>.Some(channelActor);
            return Unit.Default;
        }

        private void HandleChatMessage(AdminChatMessageEvent msg)
        {
            logger.LogInformation($"{msg.Player.Name} : {msg.Message}");
        }

        protected override void PostStop()
        {
            base.PostStop();
            var self = Self;
            chatChannel.Some(a => a.Tell(new UnregisterFromChatChannel(self)));
        }
    }
}
