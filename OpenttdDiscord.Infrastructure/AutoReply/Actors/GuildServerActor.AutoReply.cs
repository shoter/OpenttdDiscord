using Akka.Actor;
using OpenTTDAdminPort;
using OpenttdDiscord.Infrastructure.AutoReply.Actors;
using OpenttdDiscord.Infrastructure.AutoReply.Messages;

namespace OpenttdDiscord.Infrastructure.Ottd.Actors
{
    internal partial class GuildServerActor
    {
        private IActorRef autoReplyActor = default!;

        private void AutoReplyConstructor(IAdminPortClient client)
        {
            autoReplyActor = Context.ActorOf(
                AutoReplyActor.Create(
                    SP,
                    client));
        }

        private void AutoReplyReady()
        {
            ReceiveRedirect<AutoReplyMessage>(() => autoReplyActor);
        }
    }
}
