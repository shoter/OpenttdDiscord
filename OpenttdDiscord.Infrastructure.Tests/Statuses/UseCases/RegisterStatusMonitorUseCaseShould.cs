using Discord;
using LanguageExt.Pipes;
using NSubstitute.ExceptionExtensions;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Statuses.Messages;
using OpenttdDiscord.Infrastructure.Statuses.UseCases;

namespace OpenttdDiscord.Infrastructure.Tests.Statuses.UseCases
{
    public class RegisterStatusMonitorUseCaseShould
    {
        private readonly Fixture fix = new();

        private readonly IStatusMonitorRepository statusMonitorRepositorySub =
            Substitute.For<IStatusMonitorRepository>();

        private readonly IDiscordClient discordClientSub = Substitute.For<IDiscordClient>();
        private readonly IAkkaService akkaServiceSub = Substitute.For<IAkkaService>();

        private readonly IMessageChannel defaultChannelSub = Substitute.For<IMessageChannel>();
        private readonly OttdServer defaultOttdServer;
        private readonly ulong defaultGuildId;
        private readonly ulong defaultChannelId;
        private readonly IUserMessage defaultMessageSub = Substitute.For<IUserMessage>();

        public RegisterStatusMonitorUseCaseShould()
        {
            defaultOttdServer = fix.Create<OttdServer>();
            defaultGuildId = fix.Create<ulong>();
            defaultChannelId = fix.Create<ulong>();
            defaultMessageSub.Id.Returns(fix.Create<ulong>());

            // return what you received
            statusMonitorRepositorySub.Insert(Arg.Is<StatusMonitor>(sm => sm != null))
                .Returns(
                    callInfo => (StatusMonitor) callInfo.Args()
                        .First());

            discordClientSub.GetChannelAsync(defaultChannelId)
                .Returns(defaultChannelSub);

            defaultChannelSub.SendMessageAsync()
                .ReturnsForAnyArgs(defaultMessageSub);
        }

        [Fact]
        public async Task DeleteEmbeddedMessage_WhenDatabaseInsertionGoesWrong()
        {
            statusMonitorRepositorySub.Insert(
                    default!)
                .ReturnsForAnyArgs(EitherAsync<IError, StatusMonitor>.Left(Substitute.For<IError>()));

            await CreateSut()
                .Execute(
                    defaultOttdServer,
                    defaultGuildId,
                    defaultChannelId);

            await defaultChannelSub.Received()
                .DeleteMessageAsync(
                    defaultMessageSub.Id,
                    Arg.Any<RequestOptions>());
        }

        [Fact]
        public async Task DeleteEmbeddedMessage_WhenActorRegistrationInSystemGoesWrong()
        {
            akkaServiceSub.SelectAndAsk<object>(
                    default!,
                    default!)
                .ReturnsForAnyArgs(EitherAsync<IError, object>.Left(Substitute.For<IError>()));

            await CreateSut()
                .Execute(
                    defaultOttdServer,
                    defaultGuildId,
                    defaultChannelId);

            await defaultChannelSub.Received()
                .DeleteMessageAsync(
                    defaultMessageSub.Id,
                    Arg.Any<RequestOptions>());
        }

        [Fact]
        public async Task InsertStatusMonitorInfoIntoRepository()
        {
            await CreateSut()
                .Execute(
                    defaultOttdServer,
                    defaultGuildId,
                    defaultChannelId);

            await akkaServiceSub.Received()
                .SelectAndAsk<object>(
                    MainActors.Paths.Guilds,
                    Arg.Is<RegisterStatusMonitor>(
                        msg =>
                            msg.ServerId == defaultOttdServer.Id &&
                            msg.StatusMonitor.ChannelId == defaultChannelId &&
                            msg.StatusMonitor.ServerId == defaultOttdServer.Id &&
                            msg.StatusMonitor.MessageId == defaultMessageSub.Id &&
                            msg.StatusMonitor.GuildId == defaultGuildId),
                    Arg.Any<TimeSpan>());
        }

        [Fact]
        public async Task InformGuildActorAboutRegistration()
        {
            await CreateSut()
                .Execute(
                    defaultOttdServer,
                    defaultGuildId,
                    defaultChannelId);

            await statusMonitorRepositorySub.Received()
                .Insert(
                    Arg.Is<StatusMonitor>(
                        sm =>
                            sm.ServerId == defaultOttdServer.Id &&
                            sm.ChannelId == defaultChannelId &&
                            sm.MessageId == defaultMessageSub.Id &&
                            sm.GuildId == defaultGuildId
                    ));
        }

        private RegisterStatusMonitorUseCase CreateSut()
        {
            return new(
                statusMonitorRepositorySub,
                discordClientSub,
                akkaServiceSub);
        }
    }
}