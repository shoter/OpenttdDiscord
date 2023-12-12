using System.Diagnostics.CodeAnalysis;
using Akka.Actor;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Base.Fundamentals;

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
        public static EitherAsync<IError, T> TryAsk<T>(
            this ICanTell canTell,
            object msg,
            TimeSpan? timeout = null) => from result in TryAsync(
                async () => await canTell.Ask(
                    msg,
                    timeout)).ToEitherAsyncError()
            from convertedResult in TryAsyncConvert<T>(result)
            select convertedResult;

        private static EitherAsync<IError, T> TryAsyncConvert<T>(object t)
        {
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
                return EitherAsync<IError, T>.Left(error);
            }

            return t.ConvertExt<T>();
        }

        public static EitherAsync<IError, object> TryAsk(this ICanTell actor, object msg, TimeSpan? timeout = null)
            => TryAsk<object>(actor, msg, timeout);
    }
}
