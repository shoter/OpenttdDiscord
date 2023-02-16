using LanguageExt.Common;

namespace OpenttdDiscord.Base.Ext
{
    public class ExceptionError : IError
    {
        public string Reason { get; }

        public Exception Exception { get; }

        public ExceptionError(Exception ex)
        {
            this.Reason = ex.Message;
            this.Exception = ex;
        }

        public static ExceptionError FromError(Error error)
        {
            if (error.IsExceptional)
            {
                return new ExceptionError((Exception)error.Exception);
            }
            else
            {
                return new ExceptionError(new Exception(error.Message));
            }
        }
    }
}
