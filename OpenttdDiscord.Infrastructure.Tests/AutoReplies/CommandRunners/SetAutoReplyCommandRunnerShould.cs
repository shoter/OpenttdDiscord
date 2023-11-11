using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners;
using OpenttdDiscord.Infrastructure.AutoReplies.Modals;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.CommandRunners
{
    public class SetAutoReplyCommandRunnerShould : CommandRunnerTestBase
    {
        private readonly IGetAutoReplyUseCase getAutoReplyUseCaseSub = Substitute.For<IGetAutoReplyUseCase>();
        private readonly IGetServerUseCase getServerUseCaseSub = Substitute.For<IGetServerUseCase>();

        private readonly string defaultServerName;
        private readonly long defaultAction;
        private readonly string defaultTrigger;
        private readonly OttdServer defaultServer;

        public SetAutoReplyCommandRunnerShould()
        {
            defaultServerName = fix.Create<string>();
            defaultAction = (long) fix.Create<AutoReplyAction>();
            defaultTrigger = fix.Create<string>();
            defaultServer = fix.Create<OttdServer>() with { Name = defaultServerName };

            getServerUseCaseSub
                .Execute(
                    defaultServerName,
                    GuildId)
                .Returns(defaultServer);

            WithOption(
                "server-name",
                defaultServerName);
            WithOption(
                "action",
                defaultAction
            );
            WithOption(
                "trigger",
                defaultTrigger);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel)
        => await NotExecuteFor(
                CreateSut(),
                userLevel);

        [Fact]
        public async Task OpenAModal_WithNoContentForAutoReply_IfItWasNotYetCreated()
        {
            getAutoReplyUseCaseSub
                .Execute(
                    GuildId,
                    defaultServer.Id,
                    defaultTrigger)
                .Returns(Option<AutoReply>.None);

            var result = await WithGuildUser()
                .WithUserLevel(UserLevel.Admin)
                .RunExt(CreateSut());

            var value = (ModalResponse) result.Right();
            var modal = (SetAutoReplyModal) value.Modal;

            Assert.Equal(
                defaultServerName,
                modal.ServerName);
            Assert.Equal(
                defaultTrigger,
                modal.TriggerMessage);
            Assert.Equal(
                ((AutoReplyAction) defaultAction).ToString()
                .ToUpperInvariant(),
                modal.AdditionalAction.ToUpperInvariant());
            Assert.Equal(
                string.Empty,
                modal.ResponseMessage);
        }

        [Fact]
        public async Task OpenAModal_WithContentForAutoReply_IfItExistInDatabase()
        {
            var existingAutoReply = fix.Create<AutoReply>() with { TriggerMessage = defaultTrigger };

            getAutoReplyUseCaseSub
                .Execute(
                    GuildId,
                    defaultServer.Id,
                    defaultTrigger)
                .Returns(Some(existingAutoReply));

            var result = await WithGuildUser()
                .WithUserLevel(UserLevel.Admin)
                .RunExt(CreateSut());

            var value = (ModalResponse) result.Right();
            var modal = (SetAutoReplyModal) value.Modal;

            Assert.Equal(
                defaultServerName,
                modal.ServerName);
            Assert.Equal(
                defaultTrigger,
                modal.TriggerMessage);
            Assert.Equal(
                ((AutoReplyAction) defaultAction).ToString()
                .ToUpperInvariant(),
                modal.AdditionalAction.ToUpperInvariant());
            Assert.Equal(
                existingAutoReply.ResponseMessage,
                modal.ResponseMessage);
        }

        private SetAutoReplyCommandRunner CreateSut()
        {
            return new(
                AkkaServiceSub,
                GetRoleLevelUseCaseSub,
                getAutoReplyUseCaseSub,
                getServerUseCaseSub);
        }
    }
}