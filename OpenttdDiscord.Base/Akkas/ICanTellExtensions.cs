using Akka.Actor;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Base.Akkas
{
    public static class ICanTellExtensions
    {
        public static EitherAsync<IError, T> TryAsk<T>(this ICanTell canTell, object msg, TimeSpan? timeout = null)
            => TryAsync<Either<IError, T>>(async () =>
            {
                var t = await canTell.Ask(msg, timeout);

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
    }
}
