using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Servers.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Servers.Runners
{
    public class RemoveOttdServerRunnerShould : RunnerTestBase
    {
        private readonly IRemoveOttdServerUseCase removeOttdServerUseCaseSub =
            Substitute.For<IRemoveOttdServerUseCase>();

        private readonly RemoveOttdServerRunner sut;

        public RemoveOttdServerRunnerShould()
        {
            sut = new(
                removeOttdServerUseCaseSub,
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