using Akka.Actor;
using Akka.TestKit;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenttdDiscord.Domain.Chatting.Translating;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Chatting.Actors;
using OpenttdDiscord.Infrastructure.Chatting.Messages;
using OpenttdDiscord.Tests.Common.Akkas;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.Chatting
{
    public class OttdCommunicationActorShould : BaseActorTestKit
    {
        private readonly IAkkaService akkaServiceMock = Substitute.For<IAkkaService>();
        private readonly IAdminPortClient adminPortClientMock = Substitute.For<IAdminPortClient>();
        private readonly IActorRef chatManagerTester;

        private readonly ulong channelId;
        private readonly OttdServer ottdServer;

        private readonly Player validPlayer;

        private readonly IActorRef sut;
        private readonly TestProbe chatChannelProbe;

        public OttdCommunicationActorShould(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            channelId = fix.Create<ulong>();
            ottdServer = fix.Create<OttdServer>();
            validPlayer = fix.Create<Player>() with { ClientId = 2 };
            chatChannelProbe = CreateTestProbe();
            chatManagerTester = ActorOf(
                TesterActor.Create(),
                nameof(chatManagerTester));
            chatManagerTester.Ask(
                    new TesterHandle(
                        msg => msg is GetCreateChatChannel,
                        _ => chatChannelProbe))
                .Wait();

            akkaServiceMock.ReturnsActorOnSelect(
                MainActors.Paths.ChatChannelManager,
                chatManagerTester);

            sut = ActorOf(
                OttdCommunicationActor.Create(
                    Sp,
                    channelId,
                    ottdServer,
                    adminPortClientMock), nameof(sut));

            chatChannelProbe.ExpectMsg<RegisterToChatChannel>(
                (
                    _,
                    actor) =>
                {
                    actor.Tell(Unit.Default);
                });
        }

        protected override void InitializeServiceProvider(IServiceCollection services)
        {
            base.InitializeServiceProvider(services);

            services.AddSingleton(akkaServiceMock);
        }

        [Fact]
        public void SendProperMessage_FromOttd()
        {
            var msg = new AdminChatMessageEvent(
                validPlayer,
                ChatDestination.DESTTYPE_BROADCAST,
                NetworkAction.NETWORK_ACTION_SERVER_MESSAGE,
                "2137");

            sut.Tell(msg);

            var expectedMessage = new HandleOttdMessage(
                ottdServer,
                msg.Player.Name,
                msg.Message);

            chatChannelProbe
                .ExpectMsg(
                    expectedMessage);
        }

        [Fact]
        public void NotSendMessage_FromServer()
        {
            var msg = new AdminChatMessageEvent(
                validPlayer with { ClientId = 1 },
                ChatDestination.DESTTYPE_BROADCAST,
                NetworkAction.NETWORK_ACTION_SERVER_MESSAGE,
                "2137");

            sut.Tell(msg);

            var expectedMessage = new HandleOttdMessage(
                ottdServer,
                msg.Player.Name,
                msg.Message);

            chatChannelProbe
                .ExpectNoMsg();
        }

        [Fact]
        public void NotSendMessage_ThatIsNotChatMessage()
        {
            var msg = new AdminChatMessageEvent(
                validPlayer,
                ChatDestination.DESTTYPE_BROADCAST,
                NetworkAction.NETWORK_ACTION_JOIN,
                "2137");

            sut.Tell(msg);

            chatChannelProbe
                .ExpectNoMsg();
        }

        [Theory]
        [InlineData(ChatDestination.DESTTYPE_TEAM)]
        [InlineData(ChatDestination.DESTTYPE_CLIENT)]
        public void NotSendMessage_ThatIsNotBroadcast(ChatDestination invalidChatDestination)
        {
            var msg = new AdminChatMessageEvent(
                validPlayer,
                invalidChatDestination,
                NetworkAction.NETWORK_ACTION_JOIN,
                "2137");

            sut.Tell(msg);

            chatChannelProbe
                .ExpectNoMsg();
        }
    }
}