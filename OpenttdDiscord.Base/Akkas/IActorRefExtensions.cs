using System.Diagnostics.CodeAnalysis;
using Akka.Actor;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Base.Akkas
{
    public static class IActorRefExtensions
    {
        public static EitherUnit TellExt(
            this IActorRef actor,
            object msg)
        {
            actor.Tell(msg);
            return Unit.Default;
        }

        public static EitherUnit TellExt(
            this Option<IActorRef> actor,
            object msg) => actor.IfSome(actor => actor.Tell(msg));

        public static EitherUnit ForwardExt(
            this Option<IActorRef> actor,
            object msg) => actor.IfSome(actor => actor.Forward(msg));

        public static Unit TellMany<T>(
            this IActorRef actor,
            IEnumerable<T> msgs)
        {
            foreach (var msg in msgs)
            {
                actor.Tell(msg);
            }

            return Unit.Default;
        }

        public static Unit TellMany(
            this IEnumerable<IActorRef> actors,
            object msg)
        {
            foreach (var actor in actors)
            {
                actor.Tell(msg);
            }

            return Unit.Default;
        }

        [SuppressMessage(
            "Maintainability",
            "AV1500:Member or local function contains too many statements",
            Justification = "There is no need to chunk this method. It has very simple functionality")]
        public static EitherAsync<IError, T> TryAsk<T>(
            this IActorRef actor,
            object msg,
            TimeSpan? timeout = null) => TryAsync<Either<IError, T>>(
                async () =>
                {
                    var response = await actor.Ask(
                        msg,
                        timeout);

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
                })
            .ToEitherAsyncErrorFlat();

        public static EitherAsync<IError, object> TryAsk(
            this IActorRef actor,
            object msg,
            TimeSpan? timeout = null) => TryAsk<object>(
            actor,
            msg,
            timeout);
    }
}