using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.AutoReply.Messages;
using OpenttdDiscord.Infrastructure.AutoReply.UseCases;
using OpenttdDiscord.Tests.Common.Akkas;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.UseCases
{
    public class UpsertWelcomeMessageUseCaseShould : BaseActorTestKit
    {
        private readonly UpsertWelcomeMessageUseCase sut;

        private readonly IAutoReplyRepository autoReplyRepositorySubstitute = Substitute.For<IAutoReplyRepository>();

        private readonly IAkkaService akkaService = Substitute.For<IAkkaService>();

        public UpsertWelcomeMessageUseCaseShould(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            sut = new(
                autoReplyRepositorySubstitute,
                akkaService);

            autoReplyRepositorySubstitute
                .UpsertWelcomeMessage(
                    default,
                    default!)
                .ReturnsForAnyArgs(Unit.Default);

            probe.SetAutoPilot(new UnitAutoPilot());

            akkaService
                .ReturnsActorOnSelect(
                    MainActors.Paths.Guilds,
                    probe);
        }

        [Fact(Timeout = 1_000)]
        public async Task UpsertDataIntoDatabase()
        {
            ulong guildId = fix.Create<ulong>();
            Guid serverId = fix.Create<Guid>();
            string content = fix.Create<string>();

            var result = await sut.Execute(
                guildId,
                serverId,
                content);
            Assert.True(result.IsRight);

            await autoReplyRepositorySubstitute
                .Received()
                .UpsertWelcomeMessage(
                    guildId,
                    new WelcomeMessage(
                        serverId,
                        content));
        }

        [Fact(Timeout = 10_000)]
        public async Task InformActorAboutUpdate()
        {
            ulong guildId = fix.Create<ulong>();
            Guid serverId = fix.Create<Guid>();
            string content = fix.Create<string>();

            var result = await sut.Execute(
                guildId,
                serverId,
                content);
            Assert.True(result.IsRight);

            probe.ExpectMsg<UpdateWelcomeMessage>(
                x =>
                    x.GuildId == guildId &&
                        x.ServerId == serverId &&
                        x.Content == content);
        }
    }
}