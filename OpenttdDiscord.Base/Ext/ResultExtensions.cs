using System.Diagnostics.Contracts;
using LanguageExt;
using LanguageExt.Common;

#pragma warning disable SA1314 // Type parameter names should begin with T
namespace OpenttdDiscord.Base.Ext
{
    public static class ResultExtensions
    {
        [Pure]
        public static Either<L, R> Match<A, L, R>(this Result<A> result, Func<A, EitherRight<R>> Succ, Func<Exception, EitherLeft<L>> Fail)
        {
            return result.Match(
                success => Either<L, R>.Right(Succ(success)),
                fail => Either<L, R>.Left(Fail(fail))
           );
        }
    }
}
