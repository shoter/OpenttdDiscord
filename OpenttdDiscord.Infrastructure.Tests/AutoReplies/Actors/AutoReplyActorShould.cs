using Akka.Actor;
using LanguageExt.UnitsOfMeasure;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Infrastructure.AutoReply.Actors;
using OpenttdDiscord.Infrastructure.AutoReply.Messages;
using OpenttdDiscord.Tests.Common;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.Actors
{
    public class AutoReplyActorShould : BaseActorTestKit
    {
        private readonly IActorRef sut;
        private readonly IAdminPortClient adminPortClientSut = Substitute.For<IAdminPortClient>();

        public AutoReplyActorShould(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            sut = ActorOf(
                AutoReplyActor.Create(
                    Sp,
                    adminPortClientSut
                ));
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

        [Fact(Timeout = 1_000)]
        public async Task SendAMessageToJoiningPlayer_ifWelcomeMessageIsConfigured()
        {
            var welcomeMessage = fix.Create<NewWelcomeMessage>();
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
            var welcomeMessage = fix.Create<NewWelcomeMessage>();
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
                        updatedMessage.NewContent));
        }
    }
}