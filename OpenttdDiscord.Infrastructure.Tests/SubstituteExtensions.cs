using Akka.Actor;
using OpenttdDiscord.Infrastructure.Akkas;
using Array = System.Array;

namespace OpenttdDiscord.Infrastructure.Tests
{
    public static class SubstituteExtensions
    {
        public static void ReturnsActorOnSelect(
            this IAkkaService akka,
            string path,
            IActorRef actor) => akka.SelectActor(path)
            .Returns(
                new ActorSelection(
                    actor,
                    Array.Empty<string>()));
    }
}
