using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.AutoReply.Messages;
using OpenttdDiscord.Infrastructure.AutoReply.UseCases;
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

            akkaService
                .ReturnsActorOnSelect(
                    MainActors.Paths.Guilds,
                    probe);
        }

        [Fact]
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

            autoReplyRepositorySubstitute
                .Received()
                .UpsertWelcomeMessage(
                    guildId,
                    new WelcomeMessage(
                        serverId,
                        content));
        }

        [Fact]
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