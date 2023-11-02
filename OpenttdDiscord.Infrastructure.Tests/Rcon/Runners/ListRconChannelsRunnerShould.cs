using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Rcon.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Rcon.Runners
{
    public class ListRconChannelsRunnerShould : CommandRunnerTestBase
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
                AkkaServiceSub,
                GetRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        public async Task NotExecuteForNonModerator(UserLevel userLevel)
        {
            await WithGuildUser()
                .WithOption(
                    "server-name",
                    "whatever")
                .WithUserLevel(userLevel)
                .NotExecuteFor(
                    sut,
                    userLevel);
        }
    }
}