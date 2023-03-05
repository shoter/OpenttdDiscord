using Akka.Actor;

namespace OpenttdDiscord.Infrastructure.Ottd.Actors
{
    public record KillDanglingAction(IActorRef commandActor);
}
