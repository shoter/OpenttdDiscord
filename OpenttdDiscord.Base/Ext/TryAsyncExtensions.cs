using LanguageExt;

namespace OpenttdDiscord.Base.Ext
{
    public static class TryAsyncExtensions
    {
        public static EitherAsync<IError, T> ToEitherAsyncError<T>(this TryAsync<T> tryAsync)
            => tryAsync.ToEither(error => (IError)new ExceptionError(error));

        public static EitherAsync<IError, T> ToEitherAsyncErrorFlat<T>(this TryAsync<Either<IError, T>> tryAsync)
            => tryAsync
                .ToEither(error => (IError)new ExceptionError(error))
                .BiBind(
                Right: right => right.ToAsync(),
                Left: left => EitherAsync<IError, T>.Left(left)
                );
    }
}
