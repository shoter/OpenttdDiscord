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

        protected AutoReplyActor(IServiceProvider serviceProvider,
                                 IAdminPortClient client)
            : base(serviceProvider)
        {
            this.client = client;
            Ready();
        }

        private void Ready()
        {
            Receive<NewWelcomeMessage>(OnNewWelcomeMessage);
            Receive<IAdminEvent>(OnAdminEvent);
        }

        private void OnAdminEvent(IAdminEvent msg)
        {
            welcomeActor.Some(a => a.Forward(msg));
        }

        private void OnNewWelcomeMessage(NewWelcomeMessage msg)
        {
            IActorRef actor = Context.ActorOf(
                WelcomeActor.Create(
                    SP,
                    client,
                    msg.Content));

            welcomeActor = Some(actor);
        }
    }
}