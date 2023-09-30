using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Servers.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Servers.Runners
{
    public class RegisterServerRunnerShould : RunnerTestBase
    {
        private readonly IRegisterOttdServerUseCase registerOttdServerUseCaseSub =
            Substitute.For<IRegisterOttdServerUseCase>();

        private readonly RegisterServerRunner sut;

        public RegisterServerRunnerShould()
        {
            sut = new(
                registerOttdServerUseCaseSub,
                akkaServiceSub,
                getRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel)
        {
            var result = await WithGuildUser()
                .WithOption("name", "whatever")
                .WithOption("password", "dupa")
                .WithOption("port", 1234L)
                .WithOption("ip", "127.0.0.1")
                .WithUserLevel(userLevel)
                .RunExt(sut);

            Assert.True(result.IsLeft);
            Assert.True(result.Left() is IncorrectUserLevelError);
        }
    }
}
