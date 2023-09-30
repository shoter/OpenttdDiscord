using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Servers.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Servers.Runners
{
    public class ListServerRunnerShould : RunnerTestBase
    {
        private IOttdServerRepository OttdServerRepositorySub { get; } = Substitute.For<IOttdServerRepository>();

        private readonly ListServerRunner sut;

        public ListServerRunnerShould()
        {
            sut = new(
                OttdServerRepositorySub,
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
