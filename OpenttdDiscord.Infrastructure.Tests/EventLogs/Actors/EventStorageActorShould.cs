using Akka.Actor;
using FluentAssertions;
using FluentAssertions.Extensions;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Game;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.EventLogs.Actors;
using OpenttdDiscord.Infrastructure.EventLogs.Messages;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.EventLogs.Actors
{
    public class EventStorageActorShould : BaseActorTestKit
    {
        private readonly IAdminPortClient adminPortClientSut = Substitute.For<IAdminPortClient>();
        private readonly OttdServer server;

        public EventStorageActorShould(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            server = fix.Create<OttdServer>();
        }

        [Fact]
        public async Task NotHaveMoreMessagesThanLimit()
        {
            var actor = CreateSut();
            for (int i = 0; i < 650; ++i)
            {
                actor.Tell(i.ToString());
            }

            await Task.Delay(0.5.Seconds());

            var eventLog = await actor.Ask<RetrievedEventLog>(new RetrieveEventLog(server.Id, server.GuildId));
            eventLog.Messages.Count.Should()
                .Be(EventStorageActor.ChatMessageMaxCount);
        }

        private IActorRef CreateSut()
        {
            var sut = ActorOf(
                EventStorageActor.Create(
                    Sp,
                    server,
                    adminPortClientSut
                ));
            return sut;
        }
    }
}