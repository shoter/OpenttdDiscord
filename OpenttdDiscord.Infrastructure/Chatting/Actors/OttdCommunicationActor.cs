using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Chatting.Messages;
using OpenttdDiscord.Infrastructure.Ottd.Messages;
using OpenttdDiscord.Infrastructure.Rcon.Messages;

namespace OpenttdDiscord.Infrastructure.Chatting.Actors
{
    internal class OttdCommunicationActor : ReceiveActorBase
    {
        private readonly OttdServer server;
        private readonly ulong channelId;
        private readonly IAdminPortClient client;
        private readonly IAkkaService akkaService;

        private Option<IActorRef> chatChannel = Option<IActorRef>.None;

        public OttdCommunicationActor(
            IServiceProvider serviceProvider,
            ulong channelId,
            OttdServer server,
            IAdminPortClient client)
            : base(serviceProvider)
        {
            this.server = server;
            this.channelId = channelId;
            this.client = client;
            this.akkaService = SP.GetRequiredService<IAkkaService>();

            Ready();
            Self.Tell(new InitOttdCommunicationActor());
        }

        public static Props Create(IServiceProvider sp, ulong channelId, OttdServer server, IAdminPortClient client) =>
            Props.Create(() => new OttdCommunicationActor(sp, channelId, server, client));

        private void Ready()
        {
            ReceiveAsync<InitOttdCommunicationActor>(InitOttdCommunicationActor);
            Receive<HandleOttdMessage>(HandleOttdMessage);
            Receive<AdminChatMessageEvent>(HandleChatMessage);
            ReceiveIgnore<IAdminEvent>();
        }

        private async Task InitOttdCommunicationActor(InitOttdCommunicationActor _)
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

            parent.Tell(new SubscribeToAdminEvents(self));
        }

        private EitherUnit AssignChannelActor(IActorRef channelActor)
        {
            this.chatChannel = Option<IActorRef>.Some(channelActor);
            return Unit.Default;
        }

        private void HandleChatMessage(AdminChatMessageEvent msg)
        {
            if (msg.Player.ClientId == 1)
            {
                // we ignore server messages
                return;
            }

            if(msg.NetworkAction != NetworkAction.NETWORK_ACTION_SERVER_MESSAGE)
            {
                return;
            }

            this.chatChannel.TellMany(new HandleOttdMessage(server, msg.Player.Name, msg.Message));
        }

        private void HandleOttdMessage(HandleOttdMessage msg)
        {
            if (msg.Server == server)
            {
                return;
            }

            string message = $"[{msg.Server.Name}] {msg.Username}: {msg.Message}";

            client.SendMessage(new AdminChatMessage(NetworkAction.NETWORK_ACTION_CHAT, ChatDestination.DESTTYPE_BROADCAST, default, message));
            parent.Tell(msg);
        }

        protected override void PostStop()
        {
            base.PostStop();
            var self = Self;
            parent.Tell(new UnsubscribeFromAdminEvents(self));
            chatChannel.Some(a => a.Tell(new UnregisterFromChatChannel(self)));
            logger.LogInformation($"Removing Ottd communication Actor for {channelId}");
        }
    }
}
