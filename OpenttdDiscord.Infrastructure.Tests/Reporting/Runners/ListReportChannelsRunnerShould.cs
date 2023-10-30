using OpenttdDiscord.Domain.Reporting.UseCases;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Reporting.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Reporting.Runners
{
    public class ListReportChannelsRunnerShould : CommandRunnerTestBase
    {
        private readonly IGetServerUseCase getServerUseCaseSubsitute = Substitute.For<IGetServerUseCase>();

        private readonly IListReportChannelsUseCase listReportChannelsUseCaseSub =
            Substitute.For<IListReportChannelsUseCase>();

        private readonly ListReportChannelsRunner sut;

        public ListReportChannelsRunnerShould()
        {
            sut = new(
                getServerUseCaseSubsitute,
                listReportChannelsUseCaseSub,
                AkkaServiceSub,
                GetRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        public async Task NotExecuteForNonModerator(UserLevel userLevel)
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
