using Akka.Actor;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Base.Akkas
{
    public static class IActorRefExtensions
    {
        public static Unit TellMany<T>(this IActorRef actor, IEnumerable<T> msgs)
        {
            foreach (var m in msgs)
            {
                actor.Tell(m);
            }

            return Unit.Default;
        }

        public static EitherAsync<IError, T> TryAsk<T>(this ICanTell actor, object msg)
            => TryAsync<Either<IError, T>>(async () =>
            {
                var t = await actor.Ask(msg);

                if (t is Exception ex)
                {
                    return new ExceptionError(ex);
                }

                if (t is ExceptionError exe)
                {
                    return exe;
                }

                if (t is T final)
                {
                    return final;
                }

                return new ExceptionError(new Exception("Could not convert an object"));
            }).ToEitherAsyncErrorFlat();

        public static EitherAsync<IError, object> TryAsk(this ICanTell actor, object msg)
            => TryAsk<object>(actor, msg);
    }
}
