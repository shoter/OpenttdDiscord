using System.Diagnostics.CodeAnalysis;
using Akka.Actor;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Base.Akkas
{
    public static class ICanTellExtensions
    {
        public static EitherUnit TellExt(this ICanTell actor, object msg, IActorRef? sender = null)
        {
            actor.Tell(msg, sender);
            return Unit.Default;
        }

        [SuppressMessage("Maintainability",
                         "AV1500:Member or local function contains too many statements",
                         Justification = "There is no need to chunk this method. It has very simple functionality")]
        public static EitherAsync<IError, T> TryAsk<T>(this ICanTell canTell, object msg, TimeSpan? timeout = null)
            => TryAsync<Either<IError, T>>(async () =>
            {
                var response = await canTell.Ask(msg, timeout);

                if (response is Exception ex)
                {
                    return new ExceptionError(ex);
                }

                if (response is ExceptionError exe)
                {
                    return exe;
                }

                if (response is IError error)
                {
                    return Either<IError, T>.Left(error);
                }

                if (response is T final)
                {
                    return final;
                }

                return new ExceptionError(new Exception("Could not convert an object"));
            }).ToEitherAsyncErrorFlat();

        public static EitherAsync<IError, object> TryAsk(this ICanTell actor, object msg, TimeSpan? timeout = null)
            => TryAsk<object>(actor, msg, timeout);
    }
}
