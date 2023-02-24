using LanguageExt;

namespace OpenttdDiscord.Base.Ext
{
    public static class EitherAsyncExtensions
    {
        public static R Right<L, R>(this EitherAsync<L, R> either)
            => (R)either.Case.Result;

        public static L Left<L, R>(this EitherAsync<L, R> either)
            => (L)either.Case.Result;

        public static EitherAsync<IError, R> ThrowIfError<R>(this EitherAsync<IError, R> either)
        {
            if (either.IsLeft.Result)
            {
                throw new Exception(either.Left().Reason);
            }

            return either;
        }
    }
}
