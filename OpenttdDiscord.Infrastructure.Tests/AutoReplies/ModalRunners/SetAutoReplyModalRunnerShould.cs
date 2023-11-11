using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.AutoReplies.ModalRunners;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.ModalRunners
{
    public class SetAutoReplyModalRunnerShould : ModalRunnerTestBase
    {
        private readonly IUpsertAutoReplyUseCase upsertAutoReplyUseCaseSub = Substitute.For<IUpsertAutoReplyUseCase>();
        private readonly IGetServerUseCase getServerUseCaseSub = Substitute.For<IGetServerUseCase>();

        private readonly string defaultServerName;
        private readonly AutoReply defaultAutoReply;
        private readonly OttdServer defaultServer;

        public SetAutoReplyModalRunnerShould()
        {
            defaultServerName = fix.Create<string>();
            defaultAutoReply = fix.Create<AutoReply>();
            defaultServer = fix.Create<OttdServer>() with { Name = defaultServerName };

            getServerUseCaseSub.Execute(
                    defaultServerName,
                    GuildId
                )
                .Returns(defaultServer);

            upsertAutoReplyUseCaseSub
                .Execute(
                    default,
                    default,
                    default!)
                .ReturnsForAnyArgs(Unit.Default);

            WithTextInput(
                    "server-name",
                    defaultServerName)
                .WithTextInput(
                    "trigger",
                    defaultAutoReply.TriggerMessage)
                .WithTextInput(
                    "action",
                    defaultAutoReply.AdditionalAction.ToString())
                .WithTextInput(
                    "content",
                    defaultAutoReply.ResponseMessage);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel)
        {
            var sut = CreateSut();

            await WithGuildUser()
                .WithUserLevel(userLevel)
                .NotExecuteFor(
                    sut,
                    userLevel);
        }

        [Fact]
        public async Task UpdateContent_WhenContentIsSet()
        {
            var sut = CreateSut();

            var result = await WithGuildUser()
                .WithUserLevel(UserLevel.Admin)
                .RunExt(sut);

            Assert.True(result.IsRight);
            await upsertAutoReplyUseCaseSub
                .Received()
                .Execute(
                    GuildId,
                    defaultServer.Id,
                    defaultAutoReply);
        }

        private SetAutoReplyModalRunner CreateSut()
        {
            return new(
                GetRoleLevelUseCaseSub,
                upsertAutoReplyUseCaseSub,
                getServerUseCaseSub);
        }
    }
}