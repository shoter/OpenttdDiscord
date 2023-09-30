using OpenttdDiscord.Domain.EventLogs.UseCases;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.EventLogs.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.EventLogs.Runners
{
    public class QueryEventLogRunnerShould : RunnerTestBase
    {
        private readonly IGetServerUseCase getServerUseCaseSubsitute = Substitute.For<IGetServerUseCase>();

        private readonly IQueryEventLogUseCase queryEventLogUseCaseSubstitute = Substitute.For<IQueryEventLogUseCase>();

        private readonly QueryEventLogRunner sut;

        public QueryEventLogRunnerShould()
        {
            sut = new(
                getServerUseCaseSubsitute,
                queryEventLogUseCaseSubstitute,
                akkaServiceSub,
                getRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.User)]
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
