using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Rcon.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Rcon.Runners
{
    public class UnregisterRconChannelRunnerShould : RunnerTestBase
    {
        private readonly IGetServerUseCase getServerUseCaseSubsitute = Substitute.For<IGetServerUseCase>();

        private readonly IUnregisterRconChannelUseCase unregisterRconChannelUseCaseSubstitute =
            Substitute.For<IUnregisterRconChannelUseCase>();

        private readonly UnregisterRconChannelRunner sut;

        public UnregisterRconChannelRunnerShould()
        {
            sut = new(
                getServerUseCaseSubsitute,
                unregisterRconChannelUseCaseSubstitute,
                akkaServiceSub,
                getRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel)
        {
            var result = await WithGuildUser()
                .WithOption("server-name", "whatever")
                .WithUserLevel(userLevel)
                .RunExt(sut);

            Assert.True(result.IsLeft);
            Assert.True(result.Left() is IncorrectUserLevelError);
        }
    }
}
