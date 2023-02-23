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

        public ExceptionError(Error error)
        {
            Exception ex = default!;
            if (error.IsExceptional)
            {
                ex = (Exception)error.Exception;
            }
            else
            {
                ex = new Exception(error.Message);
            }

            this.Reason = ex.Message;
            this.Exception = ex;
        }
    }
}
