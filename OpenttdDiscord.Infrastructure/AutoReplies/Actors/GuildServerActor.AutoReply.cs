using Akka.Actor;
using OpenTTDAdminPort;
using OpenttdDiscord.Infrastructure.AutoReplies.Actors;
using OpenttdDiscord.Infrastructure.AutoReplies.Messages;

namespace OpenttdDiscord.Infrastructure.Ottd.Actors
{
    internal partial class GuildServerActor
    {
        private IActorRef autoReplyActor = default!;

        private void AutoReplyConstructor(IAdminPortClient client)
        {
            autoReplyActor = UntypedActor.Context.ActorOf(
                AutoReplyActor.Create(
                    SP,
                    server.GuildId,
                    server.Id,
                    client));
        }

        private void AutoReplyReady()
        {
            ReceiveForward<AutoReplyMessage>(() => autoReplyActor);
        }
    }
}
