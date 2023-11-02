using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Maintenance.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Maintenance.Runners
{
    public class HealthCheckRunnerShould : CommandRunnerTestBase
    {
        private readonly HealthCheckRunner sut;

        public HealthCheckRunnerShould()
        {
            sut = new(
                AkkaServiceSub,
                GetRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        public async Task NotExecuteForNonModerator(UserLevel userLevel)
        {
            var result = await WithGuildUser()
                .WithUserLevel(userLevel)
                .RunExt(sut);

            Assert.True(result.IsLeft);
            Assert.True(result.Left() is IncorrectUserLevelError);
        }
    }
}
