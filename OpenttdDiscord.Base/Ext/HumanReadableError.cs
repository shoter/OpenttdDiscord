using LanguageExt;
using Microsoft.Extensions.Logging;

namespace OpenttdDiscord.Base.Ext
{
    public class HumanReadableError : IError
    {
        public string Reason { get; }

        public HumanReadableError(string reason)
        {
            this.Reason = reason;
        }

        public static EitherUnit EitherUnit(string reason) => Either<IError, Unit>.Left(new HumanReadableError(reason));

        public static EitherString EitherString(string reason) => Either<IError, string>.Left(new HumanReadableError(reason));

        public static EitherLeft<IError> Left(string reason) => LanguageExt.Prelude.Left((IError)new HumanReadableError(reason));

    }
}
