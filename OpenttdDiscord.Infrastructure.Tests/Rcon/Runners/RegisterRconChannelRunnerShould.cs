using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Rcon.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Rcon.Runners
{
    public class RegisterRconChannelRunnerShould : CommandRunnerTestBase
    {
        private readonly IGetServerUseCase getServerUseCaseSubsitute = Substitute.For<IGetServerUseCase>();

        private readonly IRegisterRconChannelUseCase registerRconChannelUseCaseSubstitute =
            Substitute.For<IRegisterRconChannelUseCase>();

        private readonly IGetRconChannelUseCase getRconChannelUseCaseSub =
            Substitute.For<IGetRconChannelUseCase>();

        private readonly IGetChatChannelUseCase getChatChannelUseCaseSub = Substitute.For<IGetChatChannelUseCase>();

        private readonly RegisterRconChannelRunner sut;

        public RegisterRconChannelRunnerShould()
        {
            sut = new(
                registerRconChannelUseCaseSubstitute,
                getRconChannelUseCaseSub,
                getServerUseCaseSubsitute,
                AkkaServiceSub,
                GetRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel)
        {
            var result = await WithGuildUser()
                .WithOption(
                    "server-name",
                    "whatever")
                .WithOption(
                    "prefix",
                    "whatever")
                .WithUserLevel(userLevel)
                .RunExt(sut);

            Assert.True(result.IsLeft);
            Assert.True(result.Left() is IncorrectUserLevelError);
        }
    }
}