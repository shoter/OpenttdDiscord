using Akka.Actor;
using FluentAssertions.Common;
using FluentAssertions.Extensions;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Infrastructure.AutoReply.Actors;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.Actors
{
    public class WelcomeActorShould : BaseActorTestKit
    {
        private readonly IActorRef sut = default!;
        private readonly string initialContent;
        private IAdminPortClient adminPortClientSut = Substitute.For<IAdminPortClient>();

        public WelcomeActorShould(ITestOutputHelper outputHelper) 
            : base(outputHelper)
        {
            initialContent = fix.Create<string>();

            sut = ActorOf(
                WelcomeActor.Create(
                    Sp,
                    adminPortClientSut,
                    initialContent
                ));
        }

        [Fact]
        public void ShouldRespondToNewPlayer_WithInitialMessage()
        {
            AdminClientJoinEvent ev = fix.Create<AdminClientJoinEvent>();
            sut.Tell(ev);

            Within(
                1.Seconds(),
                () =>
                {
                    adminPortClientSut.Received()
                        .SendMessage(new AdminChatMessage(
                                         NetworkAction.NETWORK_ACTION_CHAT,
                                         ChatDestination.DESTTYPE_CLIENT,
                                         ev.Player.ClientId,
                                         initialContent));
                });
        }
    }
}