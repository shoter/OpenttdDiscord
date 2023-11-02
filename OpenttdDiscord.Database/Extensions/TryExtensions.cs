using LanguageExt;

namespace OpenttdDiscord.Database.Extensions
{
    public static class TryExtensions
    {
        public static EitherAsync<IError, T> ToEitherAsyncError<T>(this Try<T> @try) => @try
            .ToEither(error => (IError) new ExceptionError(error))
            .ToAsync();
    }
}
