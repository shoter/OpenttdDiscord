using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Chatting.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Chatting.Runner
{
    public class RegisterChatChannelRunnerShould : CommandRunnerTestBase
    {
        private readonly IGetServerUseCase getServerUseCaseSubsitute = Substitute.For<IGetServerUseCase>();

        private readonly IRegisterChatChannelUseCase registerChatChannelUseCaseSubstitute =
            Substitute.For<IRegisterChatChannelUseCase>();

        private readonly IGetChatChannelUseCase getChatChannelUseCaseSub = Substitute.For<IGetChatChannelUseCase>();

        private readonly RegisterChatChannelRunner sut;

        public RegisterChatChannelRunnerShould()
        {
            sut = new(
                getServerUseCaseSubsitute,
                registerChatChannelUseCaseSubstitute,
                getChatChannelUseCaseSub,
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