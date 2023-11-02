using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Infrastructure.Akkas.Message;
using OpenttdDiscord.Infrastructure.AutoReply.Messages;

namespace OpenttdDiscord.Infrastructure.AutoReply.Actors
{
    public class AutoReplyActor : ReceiveActorBase
    {
        private readonly IAdminPortClient client;
        private readonly IGetWelcomeMessageUseCase getWelcomeMessageUseCase;
        private readonly ulong guildId;
        private readonly Guid serverId;

        private Option<IActorRef> welcomeActor = Option<IActorRef>.None;

        public AutoReplyActor(
            IServiceProvider serviceProvider,
            ulong guildId,
            Guid serverId,
            IAdminPortClient client)
            : base(serviceProvider)
        {
            this.client = client;
            this.getWelcomeMessageUseCase = SP.GetRequiredService<IGetWelcomeMessageUseCase>();
            this.guildId = guildId;
            this.serverId = serverId;

            Ready();
            Self.Tell(new InitializeActor());
        }

        public static Props Create(
            IServiceProvider serviceProvider,
            ulong guildId,
            Guid serverId,
            IAdminPortClient client) => Props.Create(
            () => new AutoReplyActor(
                serviceProvider,
                guildId,
                serverId,
                client));

        private void Ready()
        {
            Receive<UpdateWelcomeMessage>(UpsertWelcomeMessage);
            Receive<IAdminEvent>(OnAdminEvent);
            ReceiveEitherAsync<InitializeActor>(InitializeActor);
        }

        private EitherAsyncUnit InitializeActor(InitializeActor _)
        {
            return
                from message in getWelcomeMessageUseCase.Execute(
                    guildId,
                    serverId)
                select message.IfSome(TryToInitializeActor);
        }

        private Unit TryToInitializeActor(WelcomeMessage welcomeMessage)
        {
            if (welcomeActor.IsSome)
            {
                return Unit.Default;
            }

            welcomeActor = CreateWelcomeActor(welcomeMessage.Content);
            return Unit.Default;
        }

        private void OnAdminEvent(IAdminEvent msg)
        {
            welcomeActor.TellExt(msg);
            Sender.Tell(Unit.Default);
        }

        private void UpsertWelcomeMessage(UpdateWelcomeMessage msg)
        {
            if (welcomeActor.IsSome)
            {
                welcomeActor.ForwardExt(msg);
                Sender.Tell(Unit.Default);
                return;
            }

            welcomeActor = CreateWelcomeActor(msg.Content);
            Sender.Tell(Unit.Default);
        }

        private Option<IActorRef> CreateWelcomeActor(string content)
        {
            IActorRef actor = Context.ActorOf(
                WelcomeActor.Create(
                    SP,
                    client,
                    content));

            return Some(actor);
        }
    }
}