using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.Errors;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.Errors;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners;
using OpenttdDiscord.Infrastructure.AutoReplies.Options;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Servers.Options;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.CommandRunners
{
    public class GetAutoReplyContentCommandRunnerShould : CommandRunnerTestBase
    {
        private readonly IGetServerUseCase getServerUseCaseSub = Substitute.For<IGetServerUseCase>();
        private readonly IGetAutoReplyUseCase getAutoReplyUseCaseSub = Substitute.For<IGetAutoReplyUseCase>();

        private readonly OttdServer defaultOttdServer;
        private readonly AutoReply defaultAutoReply;

        public GetAutoReplyContentCommandRunnerShould()
        {
            defaultOttdServer = fix.Create<OttdServer>();
            defaultAutoReply = fix.Create<AutoReply>();

            getServerUseCaseSub.Execute(
                    defaultOttdServer.Name,
                    GuildId)
                .Returns(defaultOttdServer);

            getAutoReplyUseCaseSub.Execute(
                    GuildId,
                    defaultOttdServer.Id,
                    defaultAutoReply.TriggerMessage)
                .Returns(Some(defaultAutoReply));

            WithOption(
                ServerNameOption.OptionName,
                defaultOttdServer.Name);
            WithOption(
                AutoReplyTriggerOption.OptionName,
                defaultAutoReply.TriggerMessage);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel) => await NotExecuteFor(
            CreateSut(),
            userLevel);

        [Fact]
        public async Task GetAutoReplyContent()
        {
            var response = await RunExt(CreateSut());

            var textResponse = (TextResponse) response.Right();
            Assert.Contains(
                defaultAutoReply.ResponseMessage,
                textResponse.Response);
        }

        [Fact]
        public async Task ReturnErrorWhenThereIsNoSuchMessage()
        {
            getAutoReplyUseCaseSub.Execute(
                    default!,
                    default,
                    default!)
                .ReturnsForAnyArgs(
                    Option<AutoReply>.None);

            var response = await RunExt(CreateSut());
            Assert.IsType<AutoReplyNotFound>(response.Left());
        }

        [Fact]
        public async Task ReturnErrorWhenThereIsNoServer()
        {
            getServerUseCaseSub.Execute(
                    default!,
                    default)
                .ReturnsForAnyArgs(
                    new ServerNotFoundError());

            var response = await RunExt(CreateSut());
            Assert.IsType<ServerNotFoundError>(response.Left());
        }

        private GetAutoReplyContentCommandRunner CreateSut()
        {
            return new(
                AkkaServiceSub,
                GetRoleLevelUseCaseSub);
        }
    }
}