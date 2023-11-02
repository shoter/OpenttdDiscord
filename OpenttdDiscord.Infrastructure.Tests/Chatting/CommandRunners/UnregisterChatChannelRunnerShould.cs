using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Chatting.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Chatting.CommandRunners
{
    public class UnregisterChatChannelRunnerShould : CommandRunnerTestBase
    {
        private readonly IGetServerUseCase getServerUseCaseSubsitute = Substitute.For<IGetServerUseCase>();

        private readonly IUnregisterChatChannelUseCase unregisterChatChannelUseCase =
            Substitute.For<IUnregisterChatChannelUseCase>();

        private readonly UnregisterChatChannelRunner sut;

        public UnregisterChatChannelRunnerShould()
        {
            sut = new(
                unregisterChatChannelUseCase,
                getServerUseCaseSubsitute,
                AkkaServiceSub,
                GetRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel)
        {
            await WithGuildUser()
                .WithOption(
                    "server-name",
                    "whatever")
                .WithUserLevel(userLevel)
                .NotExecuteFor(
                    sut,
                    userLevel);
        }
    }
}
