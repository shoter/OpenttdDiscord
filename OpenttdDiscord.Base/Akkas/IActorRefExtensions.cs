using Akka.Actor;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Base.Akkas
{
    public static class IActorRefExtensions
    {
        public static EitherUnit TellExt(this IActorRef actor, object msg)
        {
            actor.Tell(msg);
            return Unit.Default;
        }

        public static Unit TellMany<T>(this IActorRef actor, IEnumerable<T> msgs)
        {
            foreach (var m in msgs)
            {
                actor.Tell(m);
            }

            return Unit.Default;
        }

        public static Unit TellMany(this IEnumerable<IActorRef> actor, object msg)
        {
            foreach (var a in actor)
            {
                a.Tell(msg);
            }

            return Unit.Default;
        }

        public static EitherAsync<IError, T> TryAsk<T>(this IActorRef actor, object msg, TimeSpan? timeout = null)
            => TryAsync<Either<IError, T>>(async () =>
            {
                var t = await actor.Ask(msg, timeout);

                if (t is Exception ex)
                {
                    return new ExceptionError(ex);
                }

                if (t is ExceptionError exe)
                {
                    return exe;
                }

                if (t is IError error)
                {
                    return Either<IError, T>.Left(error);
                }

                if (t is T final)
                {
                    return final;
                }

                return new ExceptionError(new Exception("Could not convert an object"));
            }).ToEitherAsyncErrorFlat();

        public static EitherAsync<IError, object> TryAsk(this IActorRef actor, object msg, TimeSpan? timeout = null)
            => TryAsk<object>(actor, msg, timeout);
    }
}
