using Akka.Actor;
using FluentAssertions.Extensions;
using NSubstitute.ReceivedExtensions;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Infrastructure.AutoReplies.Actors;
using OpenttdDiscord.Infrastructure.AutoReplies.Messages;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.Actors
{
    public class AutoReplyInstanceActorShould : BaseActorTestKit
    {
        private readonly IActorRef sut;
        private readonly IAdminPortClient adminPortClientSut = Substitute.For<IAdminPortClient>();
        private readonly AutoReply defaultAutoReply;
        private readonly Player defaultPlayer;
        private readonly ServerStatus defaultStatus;

        public AutoReplyInstanceActorShould(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            defaultStatus = new(
                fix.Create<AdminServerInfo>(),
                new Dictionary<uint, Player>());
            defaultAutoReply = new(
                "!2137",
                "papaj",
                AutoReplyAction.None);

            defaultPlayer = fix.Create<Player>() with
            {
                ClientId = 2137,

                // To not be a spectator
                PlayingAs = 12,
            };
            adminPortClientSut.QueryServerStatus()
                .Returns(Task.FromResult(defaultStatus));

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
        public async Task ShouldWriteUpdatedResponse_WhenUpdated()
        {
            var msg = new AdminChatMessageEvent(
                defaultPlayer,
                ChatDestination.DESTTYPE_BROADCAST,
                NetworkAction.NETWORK_ACTION_CHAT,
                defaultAutoReply.TriggerMessage
            );

            var udpatedAutoReply = fix.Create<AutoReply>() with { TriggerMessage = defaultAutoReply.TriggerMessage };

            // IDs are not important inside this actor. They are used only for routing
            var updateMsg = new UpdateAutoReply(
                0u,
                Guid.Empty,
                udpatedAutoReply);

            sut.Tell(updateMsg);
            sut.Tell(msg);
            await Task.Delay(.5.Seconds());

            adminPortClientSut
                .Received()
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        defaultPlayer.ClientId,
                        udpatedAutoReply.ResponseMessage));
        }

        [Fact]
        public async Task ShouldChunkLongTriggerMessages()
        {
            string longMsg = fix.Create<string>();
            while (longMsg.Length < 1000)
            {
                longMsg += fix.Create<string>();
            }

            var newSut = ActorOf(
                AutoReplyInstanceActor.Create(
                    Sp,
                    defaultAutoReply with { ResponseMessage = longMsg },
                    adminPortClientSut));

            var msg = new AdminChatMessageEvent(
                defaultPlayer,
                ChatDestination.DESTTYPE_BROADCAST,
                NetworkAction.NETWORK_ACTION_CHAT,
                defaultAutoReply.TriggerMessage
            );
            newSut.Tell(msg);
            await Task.Delay(.5.Seconds());

            var calls = adminPortClientSut
                .ReceivedCalls();
            int i = 0;
            foreach (var call in calls)
            {
                var message = (call.GetArguments()
                    .Single() as AdminChatMessage)!.Message;

                foreach (var character in message)
                {
                    if (longMsg[i] == '\r' || longMsg[i] == '\n')
                    {
                        ++i;
                        continue;
                    }

                    Assert.Equal(
                        character,
                        longMsg[i]);

                    ++i;
                }
            }

            Assert.Equal(
                longMsg.Length,
                i);
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

            // players needs to be moved to spectactors.
            adminPortClientSut.Received()
                .SendMessage(new AdminRconMessage($"move {defaultPlayer.ClientId} 255"));

            adminPortClientSut.Received()
                .SendMessage(new AdminRconMessage($"reset_company {defaultPlayer.PlayingAs + 1}"));
        }

        [Fact]
        public async Task ShouldNotResetCompany_WhenThereAre2OrMorePlayersInCompany()
        {
            var autoReply = defaultAutoReply with { AdditionalAction = AutoReplyAction.ResetCompany };
            var otherPlayer = fix.Create<Player>() with { PlayingAs = defaultPlayer.PlayingAs };
            var status = new ServerStatus(
                fix.Create<AdminServerInfo>(),
                new Dictionary<uint, Player>()
                {
                    { defaultPlayer.ClientId, defaultPlayer },
                    { otherPlayer.ClientId, otherPlayer },
                });
            adminPortClientSut.QueryServerStatus()
                .Returns(Task.FromResult(status));

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

            // players needs to be moved to spectactors.
            adminPortClientSut.DidNotReceive()
                .SendMessage(new AdminRconMessage($"move {defaultPlayer.ClientId} 255"));

            adminPortClientSut.DidNotReceive()
                .SendMessage(new AdminRconMessage($"reset_company {defaultPlayer.PlayingAs + 1}"));
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