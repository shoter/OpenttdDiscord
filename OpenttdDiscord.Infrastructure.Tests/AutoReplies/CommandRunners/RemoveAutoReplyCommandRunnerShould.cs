using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners;
using OpenttdDiscord.Infrastructure.AutoReplies.Options;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Servers.Options;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.CommandRunners
{
    public class RemoveAutoReplyCommandRunnerShould : CommandRunnerTestBase
    {
        private readonly IRemoveAutoReplyUseCase removeAutoReplyUseCaseSub = Substitute.For<IRemoveAutoReplyUseCase>();

        private readonly string defaultServerName;
        private readonly string defaultTriggerMessage;

        public RemoveAutoReplyCommandRunnerShould()
        {
            defaultServerName = fix.Create<string>();
            defaultTriggerMessage = fix.Create<string>();

            WithOption(
                ServerNameOption.OptionName,
                defaultServerName);
            WithOption(
                AutoReplyTriggerOption.OptionName,
                defaultTriggerMessage);

            removeAutoReplyUseCaseSub
                .Execute(
                    GuildId,
                    defaultServerName,
                    defaultTriggerMessage)
                .Returns(Unit.Default);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel) => await NotExecuteFor(
            CreateSut(),
            userLevel);

        [Fact]
        public async Task RespondWithTextMessage_OnCorrectExecution()
        {
            var result = await WithGuildUser()
                .WithUserLevel(UserLevel.Admin)
                .RunExt(CreateSut());

            Assert.True(result.Right() is TextResponse);
        }

        [Fact]
        public async Task ReturnAnError_IfRemovalEndsWithError()
        {
            var error = Substitute.For<IError>();
            removeAutoReplyUseCaseSub
                .Execute(
                    GuildId,
                    defaultServerName,
                    defaultTriggerMessage)
                .Returns(EitherAsyncUnit.Left(error));

            var result = await WithGuildUser()
                .WithUserLevel(UserLevel.Admin)
                .RunExt(CreateSut());

            Assert.Equal(
                error,
                result.Left());
        }

        private RemoveAutoReplyCommandRunner CreateSut()
        {
            return new(
                AkkaServiceSub,
                GetRoleLevelUseCaseSub,
                removeAutoReplyUseCaseSub);
        }
    }
}