using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.AutoReplies.Messages;
using OpenttdDiscord.Infrastructure.AutoReplies.UseCases;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.UseCases
{
    public class RemoveAutoReplyUseCaseShould : BaseActorTestKit
    {
        private readonly IGetServerUseCase getServerUseCaseSub = Substitute.For<IGetServerUseCase>();
        private readonly IGetAutoReplyUseCase getAutoReplyUseCaseSub = Substitute.For<IGetAutoReplyUseCase>();
        private readonly IAutoReplyRepository autoReplyRepositorySub = Substitute.For<IAutoReplyRepository>();
        private readonly IAkkaService akkaServiceSub = Substitute.For<IAkkaService>();

        private readonly OttdServer defaultServer;
        private readonly ulong defaultGuildId;
        private readonly AutoReply defaultAutoReply;

        public RemoveAutoReplyUseCaseShould(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            this.defaultServer = fix.Create<OttdServer>();
            this.defaultGuildId = fix.Create<ulong>();
            this.defaultAutoReply = fix.Create<AutoReply>();

            getServerUseCaseSub
                .Execute(
                    defaultServer.Name,
                    defaultGuildId
                )
                .Returns(defaultServer);

            akkaServiceSub
                .SelectAndAsk(
                    default!,
                    default!,
                    default!)
                .ReturnsForAnyArgs(Unit.Default);

            autoReplyRepositorySub
                .RemoveAutoReply(
                    defaultGuildId,
                    defaultServer.Id,
                    defaultAutoReply.TriggerMessage)
                .Returns(Unit.Default);

            getAutoReplyUseCaseSub
                .Execute(
                    defaultGuildId,
                    defaultServer.Id,
                    defaultAutoReply.TriggerMessage)
                .Returns(Some(defaultAutoReply));
        }

        [Fact]
        public async Task DeleteAutoReply()
        {
            await CreateSut()
                .Execute(
                    defaultGuildId,
                    defaultServer.Name,
                    defaultAutoReply.TriggerMessage);

            await autoReplyRepositorySub
                .Received()
                .RemoveAutoReply(
                    defaultGuildId,
                    defaultServer.Id,
                    defaultAutoReply.TriggerMessage);

            await akkaServiceSub
                .Received()
                .SelectAndAsk(
                    MainActors.Paths.Guilds,
                    new RemoveAutoReply(
                        defaultGuildId,
                        defaultServer.Id,
                        defaultAutoReply.TriggerMessage));
        }

        [Fact]
        public async Task NotDeleteAutoReply_WhenServerNotFound()
        {
            var error = Substitute.For<IError>();
            getServerUseCaseSub
                .Execute(
                    defaultServer.Name,
                    defaultGuildId)
                .Returns(EitherAsync<IError, OttdServer>.Left(error));

            var result = await CreateSut()
                .Execute(
                    defaultGuildId,
                    defaultServer.Name,
                    defaultAutoReply.TriggerMessage);

            Assert.Equal(
                error,
                result.Case);
            Assert.True(result.IsLeft);

            await autoReplyRepositorySub
                .DidNotReceive()
                .RemoveAutoReply(
                    defaultGuildId,
                    defaultServer.Id,
                    defaultAutoReply.TriggerMessage);

            await akkaServiceSub
                .DidNotReceive()
                .SelectAndAsk(
                    MainActors.Paths.Guilds,
                    new RemoveAutoReply(
                        defaultGuildId,
                        defaultServer.Id,
                        defaultAutoReply.TriggerMessage));
        }

        [Fact]
        public async Task NotDeleteAutoReply_WhenAutoReplyNotFound()
        {
            getAutoReplyUseCaseSub
                .Execute(
                    defaultGuildId,
                    defaultServer.Id,
                    defaultAutoReply.TriggerMessage)
                .Returns(Option<AutoReply>.None);

            var result = await CreateSut()
                .Execute(
                    defaultGuildId,
                    defaultServer.Name,
                    defaultAutoReply.TriggerMessage);

            Assert.True(result.IsLeft);

            await autoReplyRepositorySub
                .DidNotReceive()
                .RemoveAutoReply(
                    defaultGuildId,
                    defaultServer.Id,
                    defaultAutoReply.TriggerMessage);

            await akkaServiceSub
                .DidNotReceive()
                .SelectAndAsk(
                    MainActors.Paths.Guilds,
                    new RemoveAutoReply(
                        defaultGuildId,
                        defaultServer.Id,
                        defaultAutoReply.TriggerMessage));
        }

        public RemoveAutoReplyUseCase CreateSut()
        {
            return new(
                getServerUseCaseSub,
                akkaServiceSub,
                getAutoReplyUseCaseSub,
                autoReplyRepositorySub
            );
        }
    }
}