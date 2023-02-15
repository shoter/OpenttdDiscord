using LanguageExt;

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
    }
}
