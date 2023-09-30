using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Rcon.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Rcon.Runners
{
    public class ListRconChannelsRunnerShould : RunnerTestBase
    {
        private readonly IGetServerUseCase getServerUseCaseSubsitute = Substitute.For<IGetServerUseCase>();

        private readonly IListRconChannelsUseCase listRconChannelsUseCaseSub =
            Substitute.For<IListRconChannelsUseCase>();

        private readonly ListRconChannelsRunner sut;

        public ListRconChannelsRunnerShould()
        {
            sut = new(
                getServerUseCaseSubsitute,
                listRconChannelsUseCaseSub,
                akkaServiceSub,
                getRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel)
        {
            var result = await WithGuildUser()
                .WithOption(
                    "server-name",
                    "whatever")
                .WithUserLevel(userLevel)
                .RunExt(sut);

            Assert.True(result.IsLeft);
            Assert.True(result.Left() is IncorrectUserLevelError);
        }
    }
}