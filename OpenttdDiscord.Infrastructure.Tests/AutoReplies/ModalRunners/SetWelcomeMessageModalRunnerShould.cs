using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.AutoReplies.ModalRunners;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.ModalRunners
{
    public class SetWelcomeMessageModalRunnerShould : ModalRunnerTestBase
    {
        private readonly IGetServerUseCase getServerUseCaseSub = Substitute.For<IGetServerUseCase>();

        private readonly IUpsertWelcomeMessageUseCase upsertWelcomeMessageUseCaseSub =
            Substitute.For<IUpsertWelcomeMessageUseCase>();

        private readonly SetWelcomeMessageModalRunner sut;

        private readonly string defaultServerName;
        private readonly string defaultContent;
        private readonly OttdServer defaultServer;

        public SetWelcomeMessageModalRunnerShould()
        {
            defaultServerName = fix.Create<string>();
            defaultContent = fix.Create<string>();
            defaultServer = fix.Create<OttdServer>() with
            {
                GuildId = GuildId,
                Name = defaultServerName
            };

            getServerUseCaseSub
                .Execute(
                    defaultServerName,
                    GuildId)
                .Returns(defaultServer);

            upsertWelcomeMessageUseCaseSub
                .Execute(
                    default,
                    default,
                    default!)
                .ReturnsForAnyArgs(Unit.Default);

            sut = new SetWelcomeMessageModalRunner(
                GetRoleLevelUseCaseSub,
                upsertWelcomeMessageUseCaseSub,
                getServerUseCaseSub);

            WithTextInput(
                    "server-name",
                    defaultServerName)
                .WithTextInput(
                    "content",
                    defaultContent);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel)
        {
            await WithGuildUser()
                .WithUserLevel(userLevel)
                .NotExecuteFor(
                    sut,
                    userLevel);
        }

        [Fact]
        public async Task UpdateContent_WhenContentIsSet()
        {
            var result = await WithGuildUser()
                .WithUserLevel(UserLevel.Admin)
                .RunExt(sut);

            Assert.True(result.IsRight);
            await upsertWelcomeMessageUseCaseSub
                .Received()
                .Execute(
                    GuildId,
                    defaultServer.Id,
                    defaultContent);
        }
    }
}