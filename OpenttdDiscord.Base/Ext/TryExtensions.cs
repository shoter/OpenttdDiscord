using System.Diagnostics.Contracts;
using LanguageExt;

#pragma warning disable SA1314 // Type parameter names should begin with T

namespace OpenttdDiscord.Base.Ext
{
    public static class TryExtensions
    {
        [Pure]
        public static Either<L, R> Match<A, L, R>(this Try<A> self, Func<A, Either<L, R>> Succ, Func<Exception, EitherLeft<L>> Fail)
        {
            return self.Match(
                succ => Succ(succ),
                fail => Either<L, R>.Left(Fail(fail))
                );
        }
    }
}
