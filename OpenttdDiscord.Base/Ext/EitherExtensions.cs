using LanguageExt;

namespace OpenttdDiscord.Base.Ext
{
#pragma warning disable SA1314 // Type parameter names should begin with T
    public static class EitherExtensions
    {
        public static R Right<L, R>(this Either<L, R> either)
            => (R)either.Case;

        public static L Left<L, R>(this Either<L, R> either)
            => (L)either.Case;

        public static Either<IError, R> ThrowIfError<R>(this Either<IError, R> either)
        {
            if(either.IsLeft)
            {
                throw new Exception(either.Left().Reason);
            }

            return either;
        }
    }
}
