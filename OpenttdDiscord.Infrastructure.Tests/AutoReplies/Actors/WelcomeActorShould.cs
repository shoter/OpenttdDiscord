using Akka.Actor;
using FluentAssertions.Common;
using FluentAssertions.Extensions;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Infrastructure.AutoReply.Actors;
using OpenttdDiscord.Infrastructure.AutoReply.Messages;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.Actors
{
    public class WelcomeActorShould : BaseActorTestKit
    {
        private readonly IActorRef sut;
        private readonly string initialContent;
        private readonly IAdminPortClient adminPortClientSut = Substitute.For<IAdminPortClient>();

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

        [Fact(Timeout = 1_000)]
        public async Task ShouldRespondToNewPlayer_WithInitialMessage()
        {
            AdminClientJoinEvent ev = fix.Create<AdminClientJoinEvent>();
            await sut.Ask(ev);
            adminPortClientSut.Received()
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        ev.Player.ClientId,
                        initialContent));
        }

        [Fact(Timeout = 1_000)]
        public async Task ShouldRespondToNewPlayer_WithUpdatedMessage()
        {
            // Arrange
            string updatedMessage = fix.Create<string>();
            AdminClientJoinEvent ev = fix.Create<AdminClientJoinEvent>();

            // Act
            await sut.Ask(
                new UpdateWelcomeMessage(
                    default,
                    default,
                    updatedMessage));
            await sut.Ask(ev);

            // Assert
            adminPortClientSut.Received()
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        ev.Player.ClientId,
                        updatedMessage));
        }

        [Fact(Timeout = 1_000)]
        public async Task SendMultipleMessages_IfThereAreLineBreaks()
        {
            // Arrange
            string[] separateMessages = fix.Create<string[]>();
            string updatedMessage = string.Join(
                Environment.NewLine,
                separateMessages);

            AdminClientJoinEvent ev = fix.Create<AdminClientJoinEvent>();

            // Act
            await sut.Ask(
                new UpdateWelcomeMessage(
                    default,
                    default,
                    updatedMessage));
            await sut.Ask(ev);

            // Assert
            foreach (var msg in separateMessages)
            {
                adminPortClientSut.Received()
                    .SendMessage(
                        new AdminChatMessage(
                            NetworkAction.NETWORK_ACTION_CHAT,
                            ChatDestination.DESTTYPE_CLIENT,
                            ev.Player.ClientId,
                            msg));
            }
        }
    }
}