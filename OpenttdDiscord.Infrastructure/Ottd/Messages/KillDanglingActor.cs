using Akka.Actor;

namespace OpenttdDiscord.Infrastructure.Ottd.Messages
{
    public record KillDanglingActor(IActorRef commandActor);
}
