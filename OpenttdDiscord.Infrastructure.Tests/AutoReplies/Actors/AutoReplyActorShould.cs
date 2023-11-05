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
        private readonly IActorRef sut;
        private readonly IAdminPortClient adminPortClientSut = Substitute.For<IAdminPortClient>();
        private readonly ulong guildId;
        private readonly Guid serverId;

        private readonly IGetWelcomeMessageUseCase getWelcomeMessageUseCaseSub =
            Substitute.For<IGetWelcomeMessageUseCase>();

        public AutoReplyActorShould(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            this.guildId = fix.Create<ulong>();
            this.serverId = fix.Create<Guid>();
            sut = ActorOf(
                AutoReplyActor.Create(
                    Sp,
                    guildId,
                    serverId,
                    adminPortClientSut
                ));

            getWelcomeMessageUseCaseSub
                .Execute(
                    guildId,
                    serverId)
                .Returns(Option<WelcomeMessage>.None);
        }

        protected override void InitializeServiceProvider(IServiceCollection services)
        {
            services.AddSingleton(getWelcomeMessageUseCaseSub);
        }

        [Fact(Timeout = 2_000)]
        public async Task NotSendAnything_IfPlayerJoins_AndWelcomeMessageIsNotConfigured()
        {
            var ev = fix.Create<AdminClientJoinEvent>();
            sut.Tell(ev);

            await Task.Delay(1.Seconds());
            adminPortClientSut.DidNotReceive()
                .SendMessage(
                    Arg.Is<AdminChatMessage>(
                        msg =>
                            msg.Destination == ev.Player.ClientId));
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
            var specificSut = ActorOf(
                AutoReplyActor.Create(
                    Sp,
                    guildId,
                    serverId,
                    adminPortClientSut
                ));
            await Task.Delay(.5.Seconds());

            // Act
            var ev = fix.Create<AdminClientJoinEvent>();
            await specificSut.Ask(ev);

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
        public async Task SendAMessageToJoiningPlayer_ifWelcomeMessageIsConfigured()
        {
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