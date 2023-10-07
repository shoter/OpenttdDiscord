using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Ottd.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Ottd.Runners
{
    public class QueryServerRunnerShould : RunnerTestBase
    {
        private readonly IOttdServerRepository ottdServerRepositorySub = Substitute.For<IOttdServerRepository>();

        private readonly QueryServerRunner sut;

        public QueryServerRunnerShould()
        {
            sut = new(
                ottdServerRepositorySub,
                AkkaServiceSub,
                GetRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        public async Task NotExecuteForNonModerator(UserLevel userLevel)
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