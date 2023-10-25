﻿using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Chatting.Translating;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Chatting.Messages;
using OpenttdDiscord.Infrastructure.Discord.Messages;

namespace OpenttdDiscord.Infrastructure.Chatting.Actors
{
    internal class DiscordCommunicationActor : ReceiveActorBase
    {
        private readonly ulong channelId;
        private readonly OttdServer ottdServer;
        private readonly IAdminPortClient client;
        private readonly IAkkaService akkaService;
        private readonly IChatTranslator chatTranslator;
        private Option<IActorRef> chatChannel = Option<IActorRef>.None;

        public DiscordCommunicationActor(
            IServiceProvider serviceProvider,
            ulong channelId,
            IAdminPortClient client,
            OttdServer ottdServer)
            : base(serviceProvider)
        {
            this.ottdServer = ottdServer;
            this.channelId = channelId;
            this.client = client;
            this.akkaService = SP.GetRequiredService<IAkkaService>();
            this.chatTranslator = SP.GetRequiredService<IChatTranslator>();

            Ready();
            Self.Tell(new InitDiscordChannel());
        }

        public static Props Create(IServiceProvider serviceProvider, ulong channelId, IAdminPortClient client, OttdServer ottdServer)
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
            string translated = chatTranslator
                .FromDiscordToOttd(handle.Message)
                .ThrowIfError()
                .Right();

            var msg = new AdminChatMessage(
                NetworkAction.NETWORK_ACTION_CHAT,
                ChatDestination.DESTTYPE_BROADCAST,
                default,
                $"[Discord] {handle.Username}: {translated}");

            client.SendMessage(msg);
            parent.Tell(handle);
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
            logger.LogInformation($"Removing Discord communication Actor for {channelId}");
        }
    }
}
