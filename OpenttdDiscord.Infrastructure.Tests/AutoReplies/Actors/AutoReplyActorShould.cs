using Akka.Actor;
using LanguageExt.UnitsOfMeasure;
using Microsoft.Extensions.DependencyInjection;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Infrastructure.AutoReplies.Actors;
using OpenttdDiscord.Infrastructure.AutoReplies.Messages;
using OpenttdDiscord.Tests.Common;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.Actors
{
    public class AutoReplyActorShould : BaseActorTestKit
    {
        private readonly IAdminPortClient adminPortClientSut = Substitute.For<IAdminPortClient>();
        private readonly ulong guildId;
        private readonly Guid serverId;

        private readonly IGetWelcomeMessageUseCase getWelcomeMessageUseCaseSub =
            Substitute.For<IGetWelcomeMessageUseCase>();

        private readonly IGetAutoReplyUseCase getAutoReplyUseCaseSub =
            Substitute.For<IGetAutoReplyUseCase>();

        public AutoReplyActorShould(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            this.guildId = fix.Create<ulong>();
            this.serverId = fix.Create<Guid>();

            getWelcomeMessageUseCaseSub
                .Execute(
                    guildId,
                    serverId)
                .Returns(Option<WelcomeMessage>.None);

            getAutoReplyUseCaseSub
                .Execute(
                    guildId,
                    serverId)
                .Returns(new List<AutoReply>());
        }

        protected override void InitializeServiceProvider(IServiceCollection services)
        {
            services.AddSingleton(getWelcomeMessageUseCaseSub);
            services.AddSingleton(getAutoReplyUseCaseSub);
        }

        [Fact(Timeout = 2_000)]
        public async Task NotSendAnything_IfPlayerJoins_AndWelcomeMessageIsNotConfigured()
        {
            IActorRef sut = CreateSut();
            var ev = fix.Create<AdminClientJoinEvent>();
            sut.Tell(ev);

            await Task.Delay(1.Seconds());
            adminPortClientSut.DidNotReceive()
                .SendMessage(
                    Arg.Is<AdminChatMessage>(
                        msg =>
                            msg.Destination == ev.Player.ClientId));
        }

        private IActorRef CreateSut()
        {
            var sut = ActorOf(
                AutoReplyActor.Create(
                    Sp,
                    guildId,
                    serverId,
                    adminPortClientSut
                ));
            return sut;
        }

        [Fact(Timeout = 2_000)]
        public async Task SendMessage_IfWelcomeMessage_IsConfiguredInDatabase()
        {
            var welcomeMessage = fix.Create<WelcomeMessage>();
            getWelcomeMessageUseCaseSub
                .Execute(
                    guildId,
                    serverId)
                .Returns(Some(welcomeMessage));

            // Sut needs to be recreated, because it should get data from repo at startup
            IActorRef sut = CreateSut();

            await Task.Delay(.5.Seconds());

            // Act
            var ev = fix.Create<AdminClientJoinEvent>();
            await sut.Ask(ev);

            // Assert
            adminPortClientSut.Received()
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        ev.Player.ClientId,
                        welcomeMessage.Content));
        }

        [Fact(Timeout = 2_000)]
        public async Task SendAutoReply_IfAutoReply_IsConfiguredInDatabase()
        {
            var autoReply = fix.Create<AutoReply>();

            IActorRef sut = CreateSut();

            // Act
            var ev = fix.Create<AdminChatMessageEvent>() with
            {
                Message = autoReply.TriggerMessage,
                NetworkAction = NetworkAction.NETWORK_ACTION_CHAT,
            };
            await sut.Ask(ev);

            // Assert
            adminPortClientSut.Received()
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        ev.Player.ClientId,
                        autoReply.ResponseMessage));
        }

        [Fact(Timeout = 2_000)]
        public async Task SendAutoReply_AfterReceiving_UpdateAutoReplyMessage_WhichCreatesAutoReplyActor()
        {
            var autoReply = fix.Create<AutoReply>();
            IActorRef sut = CreateSut();

            sut.Tell(new UpdateAutoReply(0, Guid.Empty, autoReply));

            // Act
            var ev = fix.Create<AdminChatMessageEvent>() with
            {
                Message = autoReply.TriggerMessage,
                NetworkAction = NetworkAction.NETWORK_ACTION_CHAT,
            };
            await sut.Ask(ev);

            // Assert
            adminPortClientSut.Received()
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        ev.Player.ClientId,
                        autoReply.ResponseMessage));
        }

        [Fact(Timeout = 1_000)]
        public async Task SendAMessageToJoiningPlayer_ifWelcomeMessageIsConfigured()
        {
            IActorRef sut = CreateSut();
            var welcomeMessage = fix.Create<UpdateWelcomeMessage>();
            await sut.Ask(welcomeMessage);

            // Act
            var ev = fix.Create<AdminClientJoinEvent>();
            await sut.Ask(ev);

            await Task.Delay(.5.Seconds());

            // Assert
            adminPortClientSut.Received()
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        ev.Player.ClientId,
                        welcomeMessage.Content));
        }

        [Fact(Timeout = 1_000)]
        public async Task SendUpdatedMessageToJoiningPlayer_ifWelcomeMessageIsConfigured()
        {
            IActorRef sut = CreateSut();
            var welcomeMessage = fix.Create<UpdateWelcomeMessage>();
            await sut.Ask(welcomeMessage);

            // Act
            var updatedMessage = fix.Create<UpdateWelcomeMessage>();
            await sut.Ask(updatedMessage);

            var ev = fix.Create<AdminClientJoinEvent>();
            await sut.Ask(ev);

            // Assert
            adminPortClientSut.Received()
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        ev.Player.ClientId,
                        updatedMessage.Content));
        }
    }
}