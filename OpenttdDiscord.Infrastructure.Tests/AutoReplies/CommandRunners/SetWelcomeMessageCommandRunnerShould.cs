using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners;
using OpenttdDiscord.Infrastructure.AutoReplies.Modals;
using OpenttdDiscord.Infrastructure.Chatting.Runners;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.CommandRunners
{
    public class SetWelcomeMessageCommandRunnerShould : CommandRunnerTestBase
    {
        private readonly IGetServerUseCase getServerUseCaseSub = Substitute.For<IGetServerUseCase>();

        private readonly IAutoReplyRepository autoReplyRepositorySub = Substitute.For<IAutoReplyRepository>();

        private readonly SetWelcomeMessageCommandRunner sut;

        public SetWelcomeMessageCommandRunnerShould()
        {
            sut = new(
                AkkaServiceSub,
                GetRoleLevelUseCaseSub,
                autoReplyRepositorySub,
                getServerUseCaseSub);
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

        [Fact]
        public async Task OpenAModal_WithNoneWelcomeMessage_IfItWasNotYetCreated()
        {
            var serverName = fix.Create<string>();
            var server = fix.Create<OttdServer>();

            getServerUseCaseSub
                .Execute(
                    serverName,
                    GuildId)
                .Returns(server);

            autoReplyRepositorySub
                .GetWelcomeMessage(
                    GuildId,
                    server.Id)
                .Returns(Option<WelcomeMessage>.None);

            var result = await WithGuildUser()
                .WithOption(
                    "server-name",
                    serverName)
                .WithUserLevel(UserLevel.Admin)
                .RunExt(sut);

            var value = (ModalResponse) result.Right();
            var modal = (SetWelcomeMessageModal) value.Modal;

            Assert.Equal(
                serverName,
                modal.ServerName);
            Assert.Equal(
                string.Empty,
                modal.InitialWelcomeMessage);
        }

        [Fact]
        public async Task OpenAModal_WithWelcomeMessage_IfWelcomeMessageIsPresent()
        {
            var serverName = fix.Create<string>();
            var server = fix.Create<OttdServer>();
            var welcomeMessage = fix.Create<WelcomeMessage>() with { ServerId = server.Id };

            getServerUseCaseSub
                .Execute(
                    serverName,
                    GuildId)
                .Returns(server);

            autoReplyRepositorySub
                .GetWelcomeMessage(
                    GuildId,
                    server.Id)
                .Returns(Some(welcomeMessage));

            var result = await WithGuildUser()
                .WithOption(
                    "server-name",
                    serverName)
                .WithUserLevel(UserLevel.Admin)
                .RunExt(sut);

            var value = (ModalResponse) result.Right();
            var modal = (SetWelcomeMessageModal) value.Modal;

            Assert.Equal(
                serverName,
                modal.ServerName);
            Assert.Equal(
                welcomeMessage.Content,
                modal.InitialWelcomeMessage);
        }
    }
}