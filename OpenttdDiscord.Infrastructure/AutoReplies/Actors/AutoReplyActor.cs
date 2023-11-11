using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Infrastructure.Akkas.Message;
using OpenttdDiscord.Infrastructure.AutoReplies.Messages;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.AutoReplies.Actors
{
    public class AutoReplyActor : ReceiveActorBase
    {
        private readonly IAdminPortClient client;
        private readonly IGetWelcomeMessageUseCase getWelcomeMessageUseCase;
        private readonly IGetAutoReplyUseCase getAutoReplyUseCase;
        private readonly ulong guildId;
        private readonly Guid serverId;
        private readonly Dictionary<string, IActorRef> instanceActors = new();

        private Option<IActorRef> welcomeActor = Option<IActorRef>.None;

        public AutoReplyActor(
            IServiceProvider serviceProvider,
            ulong guildId,
            Guid serverId,
            IAdminPortClient client)
            : base(serviceProvider)
        {
            this.client = client;
            this.getAutoReplyUseCase = SP.GetRequiredService<IGetAutoReplyUseCase>();
            this.getWelcomeMessageUseCase = SP.GetRequiredService<IGetWelcomeMessageUseCase>();
            this.guildId = guildId;
            this.serverId = serverId;

            Ready();
            InitializeActor()
                .AsTask()
                .Wait();
            parent.Tell(new SubscribeToAdminEvents(Self));
        }

        protected override void PostStop()
        {
            parent.Tell(new UnsubscribeFromAdminEvents(Self));
            base.PostStop();
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
            Receive<UpdateAutoReply>(UpdateAutoReply);
            ReceiveRedirect<AdminClientJoinEvent>(() => welcomeActor);
            ReceiveRedirect<AdminChatMessageEvent>(() => instanceActors.Values);
            ReceiveIgnore<IAdminEvent>();
        }

        private void UpdateAutoReply(UpdateAutoReply msg)
        {
            logger.LogInformation($"Updating auto reply - {msg.AutoReply}");
            var ar = msg.AutoReply;
            if (instanceActors.TryGetValue(
                    ar.TriggerMessage,
                    out var actor))
            {
                logger.LogInformation($"Actor already exists for {msg.AutoReply.TriggerMessage}. Forwarding message");
                actor.Forward(msg);
                return;
            }

            logger.LogInformation($"Actor does not exist for {msg.AutoReply.TriggerMessage}. Creating new actor");
            var newActor = Context.ActorOf(
                AutoReplyInstanceActor.Create(
                    SP,
                    ar,
                    client));
            instanceActors.Add(
                ar.TriggerMessage,
                newActor);
            logger.LogInformation($"Created actor for {msg.AutoReply.TriggerMessage}");
        }

        private EitherAsyncUnit InitializeActor()
        {
            var context = Context;
            return
                from message in getWelcomeMessageUseCase.Execute(
                    guildId,
                    serverId)
                from _1 in TryToInitializeWelcomeActor(
                    message,
                    context)
                from autoReplies in getAutoReplyUseCase.Execute(
                    guildId,
                    serverId)
                from _2 in InitializeAutoReplyActors(autoReplies, context)
                select Unit.Default;
        }

        private EitherAsyncUnit TryToInitializeWelcomeActor(
            Option<WelcomeMessage> welcomeMessage,
            IActorContext context)
        {
            if (welcomeActor.IsSome)
            {
                return Unit.Default;
            }

            return welcomeMessage.IfSome(
                msg =>
                    welcomeActor = CreateWelcomeActor(
                        msg.Content,
                        context));
        }

        private EitherAsyncUnit InitializeAutoReplyActors(
            IEnumerable<AutoReply> autoReplies,
            IActorContext context)
        {
            foreach (var ar in autoReplies)
            {
                var actor = context.ActorOf(
                    AutoReplyInstanceActor.Create(
                        SP,
                        ar,
                        client));

                instanceActors.Add(
                    ar.TriggerMessage,
                    actor);
            }

            return Unit.Default;
        }

        private void UpsertWelcomeMessage(UpdateWelcomeMessage msg)
        {
            if (welcomeActor.IsSome)
            {
                welcomeActor.ForwardExt(msg);
                Sender.Tell(Unit.Default);
                return;
            }

            welcomeActor = CreateWelcomeActor(
                msg.Content,
                Context);
            Sender.Tell(Unit.Default);
        }

        private Option<IActorRef> CreateWelcomeActor(
            string content,
            IActorContext context)
        {
            IActorRef actor = context.ActorOf(
                WelcomeActor.Create(
                    SP,
                    client,
                    content));

            return Some(actor);
        }
    }
}