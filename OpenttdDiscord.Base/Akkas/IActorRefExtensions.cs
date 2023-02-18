using Akka.Actor;

namespace OpenttdDiscord.Base.Akkas
{
    public static class IActorRefExtensions
    {
        public static void TellMany<T>(this IActorRef actor, IEnumerable<T> msgs)
        {
            foreach(var m in msgs)
            {
                actor.Tell(m);
            }
        }
    }
}
