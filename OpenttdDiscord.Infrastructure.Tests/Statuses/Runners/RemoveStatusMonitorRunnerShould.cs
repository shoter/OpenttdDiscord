using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Statuses.UseCases;
using OpenttdDiscord.Infrastructure.Statuses.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Statuses.Runners
{
    public class RemoveStatusMonitorRunnerShould : CommandRunnerTestBase
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