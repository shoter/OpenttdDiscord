using Akka.Actor;
using LanguageExt;

namespace OpenttdDiscord.Base.Akkas
{
    public static class IActorRefExtensions
    {
        public static Unit TellMany<T>(this IActorRef actor, IEnumerable<T> msgs)
        {
            foreach(var m in msgs)
            {
                actor.Tell(m);
            }

            return Unit.Default;
        }
    }
}
