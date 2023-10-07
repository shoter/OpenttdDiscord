using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Statuses.UseCases;
using OpenttdDiscord.Infrastructure.Statuses.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Statuses.Runners
{
    public class RemoveStatusMonitorRunnerShould : RunnerTestBase
    {
        private readonly IRemoveStatusMonitorUseCase removeStatusMonitorUseCaseSub =
            Substitute.For<IRemoveStatusMonitorUseCase>();

        private readonly IOttdServerRepository ottdServerRepositorySub = Substitute.For<IOttdServerRepository>();

        private readonly RemoveStatusMonitorRunner sut;

        public RemoveStatusMonitorRunnerShould()
        {
            sut = new(
                removeStatusMonitorUseCaseSub,
                ottdServerRepositorySub,
                AkkaServiceSub,
                GetRoleLevelUseCaseSub);
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
