using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Statuses.UseCases;
using OpenttdDiscord.Infrastructure.Statuses.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Statuses.Runners
{
    public class RegisterStatusMonitorRunnerShould : CommandRunnerTestBase
    {
        private readonly IRegisterStatusMonitorUseCase registerStatusMonitorUseCaseSub =
            Substitute.For<IRegisterStatusMonitorUseCase>();

        private readonly ICheckIfStatusMonitorExistsUseCase checkIfStatusMonitorExistsUseCaseSub =
            Substitute.For<ICheckIfStatusMonitorExistsUseCase>();

        private readonly IOttdServerRepository ottdServerRepositorySub = Substitute.For<IOttdServerRepository>();

        private readonly RegisterStatusMonitorRunner sut;

        public RegisterStatusMonitorRunnerShould()
        {
            sut = new(
                ottdServerRepositorySub,
                registerStatusMonitorUseCaseSub,
                checkIfStatusMonitorExistsUseCaseSub,
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
