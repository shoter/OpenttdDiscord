using LanguageExt;

namespace OpenttdDiscord.Base.Ext
{
    public class HumanReadableError : IError
    {
        public string Reason { get; }

        public HumanReadableError(string reason)
        {
            this.Reason = reason;
            var a = 5;
        }

        public static EitherUnit EitherUnit(string reason) => Either<IError, Unit>.Left(new HumanReadableError(reason));

        public static EitherLeft<IError> Left(string reason) => LanguageExt.Prelude.Left((IError) new HumanReadableError(reason));
    }
}
