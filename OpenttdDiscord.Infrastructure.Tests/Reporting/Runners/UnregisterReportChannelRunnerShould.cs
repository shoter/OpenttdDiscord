using OpenttdDiscord.Domain.Reporting.UseCases;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Reporting.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Reporting.Runners
{
    public class UnregisterReportChannelRunnerShould : CommandRunnerTestBase
    {
        private readonly IGetServerUseCase getServerUseCaseSubsitute = Substitute.For<IGetServerUseCase>();

        private readonly IUnregisterReportChannelUseCase unregisterReportChannelUseCaseSub =
            Substitute.For<IUnregisterReportChannelUseCase>();

        private readonly UnregisterReportChannelRunner sut;

        public UnregisterReportChannelRunnerShould()
        {
            sut = new(
                getServerUseCaseSubsitute,
                unregisterReportChannelUseCaseSub,
                AkkaServiceSub,
                GetRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel)
        {
            await WithGuildUser()
                .WithOption("server-name", "whatever")
                .WithUserLevel(userLevel)
                .NotExecuteFor(
                    sut,
                    userLevel);
        }
    }
}
