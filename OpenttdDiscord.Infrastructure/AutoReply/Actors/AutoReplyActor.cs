using Akka.Actor;
using LanguageExt;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Infrastructure.AutoReply.Messages;

namespace OpenttdDiscord.Infrastructure.AutoReply.Actors
{
    public class AutoReplyActor : ReceiveActorBase
    {
        private readonly IAdminPortClient client;
        private Option<IActorRef> welcomeActor = Option<IActorRef>.None;

        public AutoReplyActor(
            IServiceProvider serviceProvider,
            IAdminPortClient client)
            : base(serviceProvider)
        {
            this.client = client;
            Ready();
        }

        public static Props Create(
            IServiceProvider serviceProvider,
            IAdminPortClient client) => Props.Create(
            () => new AutoReplyActor(
                serviceProvider,
                client));

        private void Ready()
        {
            Receive<UpdateWelcomeMessage>(UpsertWelcomeMessage);
            Receive<IAdminEvent>(OnAdminEvent);
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

            IActorRef actor = Context.ActorOf(
                WelcomeActor.Create(
                    SP,
                    client,
                    msg.Content));

            welcomeActor = Some(actor);
            Sender.Tell(Unit.Default);
        }
    }
}