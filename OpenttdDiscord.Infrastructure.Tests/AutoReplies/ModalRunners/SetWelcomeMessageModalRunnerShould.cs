using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.AutoReply.ModalRunners;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.ModalRunners
{
    public class SetWelcomeMessageModalRunnerShould : ModalRunnerTestBase
    {
        private readonly IGetServerUseCase getServerUseCaseSub = Substitute.For<IGetServerUseCase>();

        private readonly IUpsertWelcomeMessageUseCase upsertWelcomeMessageUseCaseSub =
            Substitute.For<IUpsertWelcomeMessageUseCase>();

        private readonly SetWelcomeMessageModalRunner sut;

        private readonly string defaultServerName;
        private readonly string defaultContent;

        public SetWelcomeMessageModalRunnerShould()
        {
            defaultServerName = fix.Create<string>();
            defaultContent = fix.Create<string>();

            sut = new SetWelcomeMessageModalRunner(
                GetRoleLevelUseCaseSub,
                upsertWelcomeMessageUseCaseSub,
                getServerUseCaseSub);

            WithTextInput(
                    "server-name",
                    defaultServerName)
                .WithTextInput(
                    "content",
                    defaultContent);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel)
        {
            await WithGuildUser()
                .WithUserLevel(userLevel)
                .NotExecuteFor(
                    sut,
                    userLevel);
        }
    }
}