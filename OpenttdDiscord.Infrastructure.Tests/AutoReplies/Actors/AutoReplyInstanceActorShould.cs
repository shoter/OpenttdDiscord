using Akka.Actor;
using FluentAssertions.Extensions;
using NSubstitute.ReceivedExtensions;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Infrastructure.AutoReplies.Actors;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.Actors
{
    public class AutoReplyInstanceActorShould : BaseActorTestKit
    {
        private readonly IActorRef sut;
        private readonly IAdminPortClient adminPortClientSut = Substitute.For<IAdminPortClient>();
        private readonly AutoReply defaultAutoReply;
        private readonly Player defaultPlayer;

        public AutoReplyInstanceActorShould(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            defaultAutoReply = new(
                "!2137",
                "papaj",
                AutoReplyAction.None);

            defaultPlayer = fix.Create<Player>() with { ClientId = 2137 };

            sut = ActorOf(
                AutoReplyInstanceActor.Create(
                    Sp,
                    defaultAutoReply,
                    adminPortClientSut));
        }

        [Fact]
        public async Task ShouldWriteResponse_WhenTriggered_ByTriggerMessage()
        {
            var msg = new AdminChatMessageEvent(
                defaultPlayer,
                ChatDestination.DESTTYPE_BROADCAST,
                NetworkAction.NETWORK_ACTION_CHAT,
                defaultAutoReply.TriggerMessage
            );
            sut.Tell(msg);
            await Task.Delay(.5.Seconds());

            AssertSutSendResponse();
        }

        [Fact]
        public async Task ShouldResetCompany_WhenAdditionalAction_SetToResetCompany()
        {
            var autoReply = defaultAutoReply with { AdditionalAction = AutoReplyAction.ResetCompany };
            var newSut = ActorOf(
                AutoReplyInstanceActor.Create(
                    Sp,
                    autoReply,
                    adminPortClientSut));
            var msg = new AdminChatMessageEvent(
                defaultPlayer,
                ChatDestination.DESTTYPE_BROADCAST,
                NetworkAction.NETWORK_ACTION_CHAT,
                defaultAutoReply.TriggerMessage
            );
            newSut.Tell(msg);
            await Task.Delay(.5.Seconds());

            adminPortClientSut.Received()
                .SendMessage(new AdminRconMessage($"reset_company {defaultPlayer.PlayingAs}"));
        }

        [Fact]
        public async Task ShouldNotWriteResponse_WhenMessage_IsComingFromServer()
        {
            var msg = new AdminChatMessageEvent(
                new Player(
                    1,
                    "admin",
                    "admin",
                    0,
                    DateTimeOffset.Now),
                ChatDestination.DESTTYPE_BROADCAST,
                NetworkAction.NETWORK_ACTION_CHAT,
                defaultAutoReply.TriggerMessage
            );
            sut.Tell(msg);
            await Task.Delay(.5.Seconds());

            AssertSutDidNotSendResponse();
        }

        [Fact]
        public async Task ShouldNotWriteResponse_WhenMessage_IsNotTriggerMessage()
        {
            var msg = new AdminChatMessageEvent(
                defaultPlayer,
                ChatDestination.DESTTYPE_BROADCAST,
                NetworkAction.NETWORK_ACTION_CHAT,
                "!kremowka"
            );
            sut.Tell(msg);
            await Task.Delay(.5.Seconds());

            AssertSutDidNotSendResponse();
        }

        private void AssertSutSendResponse()
        {
            adminPortClientSut
                .Received()
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        defaultPlayer.ClientId,
                        defaultAutoReply.ResponseMessage));
        }

        private void AssertSutDidNotSendResponse()
        {
            adminPortClientSut
                .DidNotReceive()
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        defaultPlayer.ClientId,
                        defaultAutoReply.ResponseMessage));
        }
    }
}