using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using Array = System.Array;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.CommandRunners
{
    public class GetAutoRepliesCommandRunnerShould : CommandRunnerTestBase
    {
        private readonly IGetAutoReplyUseCase getAutoReplyUseCaseSub = Substitute.For<IGetAutoReplyUseCase>();
        private readonly IGetServerUseCase getServerUseCaseSub = Substitute.For<IGetServerUseCase>();

        private readonly string defaultServerName;
        private readonly IReadOnlyCollection<AutoReply> defaultAutoReplies;
        private readonly OttdServer defaultServer;

        public GetAutoRepliesCommandRunnerShould()
        {
            defaultServerName = fix.Create<string>();
            defaultServer = fix.Create<OttdServer>() with
            {
                Name = defaultServerName,
                GuildId = GuildId,
            };
            defaultAutoReplies = fix.Create<List<AutoReply>>();

            getServerUseCaseSub
                .Execute(
                    defaultServerName,
                    GuildId)
                .Returns(defaultServer);

            getAutoReplyUseCaseSub
                .Execute(
                    GuildId,
                    defaultServer.Id)
                .Returns(EitherAsync<IError, IReadOnlyCollection<AutoReply>>.Right(defaultAutoReplies));

            WithOption(
                "server-name",
                defaultServerName);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel) => await NotExecuteFor(
            CreateSut(),
            userLevel);

        [Fact]
        public async Task ReturnAutoReplies_FromUseCase()
        {
            EitherAsyncUnit CheckResponse(IInteractionResponse interactionResponse)
            {
                var textResponse = interactionResponse as TextResponse;
                var value = textResponse!.Response;
                foreach (var autoReply in defaultAutoReplies)
                {
                    var lines = value.Split('\n');
                    var line = lines.Single(x => x.Equals(autoReply.TriggerMessage));
                    Assert.Contains(
                        autoReply.AdditionalAction.ToString(),
                        line,
                        StringComparison.InvariantCultureIgnoreCase);
                }

                return Unit.Default;
            }

            await (from response in RunExt(CreateSut())
                from _1 in CheckResponse(response)
                select Unit.Default);
        }

        [Fact]
        public async Task ReturnNoRepliesMesssage_WhenThereIsNoReplies()
        {
            EitherAsyncUnit CheckResponse(IInteractionResponse interactionResponse)
            {
                var textResponse = interactionResponse as TextResponse;
                var value = textResponse!.Response;
                Assert.Equal(
                    GetAutoRepliesCommandRunner.NoRepliesResponse,
                    value);
                return Unit.Default;
            }

            getAutoReplyUseCaseSub
                .Execute(
                    GuildId,
                    defaultServer.Id)
                .Returns(Array.Empty<AutoReply>());

            await (from response in RunExt(CreateSut())
                from _1 in CheckResponse(response)
                select Unit.Default);
        }

        private GetAutoRepliesCommandRunner CreateSut()
        {
            return new(
                AkkaServiceSub,
                GetRoleLevelUseCaseSub,
                getAutoReplyUseCaseSub,
                getServerUseCaseSub);
        }
    }
}