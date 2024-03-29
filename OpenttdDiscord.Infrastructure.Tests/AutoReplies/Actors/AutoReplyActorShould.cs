using Akka.Actor;
using LanguageExt.Pipes;
using LanguageExt.UnitsOfMeasure;
using Microsoft.Extensions.DependencyInjection;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Infrastructure.AutoReplies.Actors;
using OpenttdDiscord.Infrastructure.AutoReplies.Messages;
using OpenttdDiscord.Tests.Common;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.AutoReplies.Actors
{
    public class AutoReplyActorShould : BaseActorTestKit
    {
        private readonly IAdminPortClient adminPortClientSut = Substitute.For<IAdminPortClient>();
        private readonly ulong defaultGuildId;
        private readonly Guid defaultServerId;

        private readonly IGetWelcomeMessageUseCase getWelcomeMessageUseCaseSub =
            Substitute.For<IGetWelcomeMessageUseCase>();

        private readonly IGetAutoReplyUseCase getAutoReplyUseCaseSub =
            Substitute.For<IGetAutoReplyUseCase>();

        public AutoReplyActorShould(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            this.defaultGuildId = fix.Create<ulong>();
            this.defaultServerId = fix.Create<Guid>();

            var serverStatus = new ServerStatus(
                fix.Create<AdminServerInfo>(),
                new Dictionary<uint, Player>());
            adminPortClientSut.QueryServerStatus()
                .Returns(Task.FromResult(serverStatus));

            getWelcomeMessageUseCaseSub
                .Execute(
                    defaultGuildId,
                    defaultServerId)
                .Returns(Option<WelcomeMessage>.None);

            getAutoReplyUseCaseSub
                .Execute(
                    defaultGuildId,
                    defaultServerId)
                .Returns(new List<AutoReply>());
        }

        protected override void InitializeServiceProvider(IServiceCollection services)
        {
            services.AddSingleton(getWelcomeMessageUseCaseSub);
            services.AddSingleton(getAutoReplyUseCaseSub);
        }

        [Fact(Timeout = 2_000)]
        public async Task NotSendAnything_IfPlayerJoins_AndWelcomeMessageIsNotConfigured()
        {
            IActorRef sut = CreateSut();
            var ev = fix.Create<AdminClientJoinEvent>();
            sut.Tell(ev);

            await Task.Delay(1.Seconds());
            adminPortClientSut.DidNotReceive()
                .SendMessage(
                    Arg.Is<AdminChatMessage>(
                        msg =>
                            msg.Destination == ev.Player.ClientId));
        }

        private IActorRef CreateSut()
        {
            var sut = ActorOf(
                AutoReplyActor.Create(
                    Sp,
                    defaultGuildId,
                    defaultServerId,
                    adminPortClientSut
                ));
            return sut;
        }

        [Fact(Timeout = 3_000)]
        public async Task SendMessage_IfWelcomeMessage_IsConfiguredInDatabase()
        {
            var welcomeMessage = fix.Create<WelcomeMessage>();
            getWelcomeMessageUseCaseSub
                .Execute(
                    defaultGuildId,
                    defaultServerId)
                .Returns(Some(welcomeMessage));

            // Sut needs to be recreated, because it should get data from repo at startup
            IActorRef sut = CreateSut();

            await Task.Delay(.5.Seconds());

            // Act
            var ev = fix.Create<AdminClientJoinEvent>();
            await sut.Ask(ev);

            // Assert
            adminPortClientSut.Received()
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        ev.Player.ClientId,
                        welcomeMessage.Content));
        }

        [Fact(Timeout = 3_000)]
        public async Task SendAutoReply_IfAutoReply_IsConfiguredInDatabase()
        {
            var autoReply = fix.Create<AutoReply>();
            getAutoReplyUseCaseSub.Execute(
                    defaultGuildId,
                    defaultServerId)
                .Returns(
                    EitherAsync<IError, IReadOnlyCollection<AutoReply>>.Right(
                        new List<AutoReply>() { autoReply }));

            IActorRef sut = CreateSut();

            // Act
            var ev = fix.Create<AdminChatMessageEvent>() with
            {
                Message = autoReply.TriggerMessage,
                NetworkAction = NetworkAction.NETWORK_ACTION_CHAT,
            };
            await sut.Ask(ev);

            // Assert
            adminPortClientSut.Received(requiredNumberOfCalls: 1)
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        ev.Player.ClientId,
                        autoReply.ResponseMessage));
        }

        [Fact(Timeout = 3_000)]
        public async Task NotSendAutoReply_IfAutoReply_IsRemovedAfterBeingConfiguredInDatabase()
        {
            var autoReply = fix.Create<AutoReply>();
            getAutoReplyUseCaseSub.Execute(
                    defaultGuildId,
                    defaultServerId)
                .Returns(
                    EitherAsync<IError, IReadOnlyCollection<AutoReply>>.Right(
                        new List<AutoReply>() { autoReply }));

            IActorRef sut = CreateSut();
            sut.Tell(
                new RemoveAutoReply(
                    defaultGuildId,
                    defaultServerId,
                    autoReply.TriggerMessage));

            // Act
            var ev = fix.Create<AdminChatMessageEvent>() with
            {
                Message = autoReply.TriggerMessage,
                NetworkAction = NetworkAction.NETWORK_ACTION_CHAT,
            };

            var response = await sut.Ask(ev);

            // Assert
            Assert.True(response is NoResponseForMessage);
            adminPortClientSut.DidNotReceive()
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        ev.Player.ClientId,
                        autoReply.ResponseMessage));
        }

        [Fact(Timeout = 2_000)]
        public async Task SendAutoReply_AfterReceiving_UpdateAutoReplyMessage_WhichCreatesAutoReplyActor()
        {
            var autoReply = fix.Create<AutoReply>();
            IActorRef sut = CreateSut();

            sut.Tell(
                new UpdateAutoReply(
                    0,
                    Guid.Empty,
                    autoReply));

            // Act
            var ev = fix.Create<AdminChatMessageEvent>() with
            {
                Message = autoReply.TriggerMessage,
                NetworkAction = NetworkAction.NETWORK_ACTION_CHAT,
            };
            await sut.Ask(ev);

            // Assert
            adminPortClientSut.Received()
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        ev.Player.ClientId,
                        autoReply.ResponseMessage));
        }

        [Fact(Timeout = 1_000)]
        public async Task SendAMessageToJoiningPlayer_ifWelcomeMessageIsConfigured()
        {
            IActorRef sut = CreateSut();
            var welcomeMessage = fix.Create<UpdateWelcomeMessage>();
            await sut.Ask(welcomeMessage);

            // Act
            var ev = fix.Create<AdminClientJoinEvent>();
            await sut.Ask(ev);

            await Task.Delay(.5.Seconds());

            // Assert
            adminPortClientSut.Received()
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        ev.Player.ClientId,
                        welcomeMessage.Content));
        }

        [Fact(Timeout = 1_000)]
        public async Task SendUpdatedMessageToJoiningPlayer_ifWelcomeMessageIsConfigured()
        {
            IActorRef sut = CreateSut();
            var welcomeMessage = fix.Create<UpdateWelcomeMessage>();
            await sut.Ask(welcomeMessage);

            // Act
            var updatedMessage = fix.Create<UpdateWelcomeMessage>();
            await sut.Ask(updatedMessage);

            var ev = fix.Create<AdminClientJoinEvent>();
            await sut.Ask(ev);

            // Assert
            adminPortClientSut.Received()
                .SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        ev.Player.ClientId,
                        updatedMessage.Content));
        }
    }
}