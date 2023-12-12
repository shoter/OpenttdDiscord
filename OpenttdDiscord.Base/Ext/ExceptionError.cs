using LanguageExt.Common;
using Microsoft.Extensions.Logging;

namespace OpenttdDiscord.Base.Ext
{
    public class ExceptionError : IError
    {
        public string Reason { get; }

        public Exception Exception { get; }

        public ExceptionError(Exception exception)
        {
            this.Reason = exception.Message;
            this.Exception = exception;
        }

        public ExceptionError(Error error)
        {
            Exception exception = default!;
            if (error.IsExceptional)
            {
                exception = (Exception)error.Exception;
            }
            else
            {
                exception = new Exception(error.Message);
            }

            this.Reason = exception.Message;
            this.Exception = exception;
        }

        public void LogError(ILogger logger)
        {
            logger.LogError(Exception, Reason);
        }
    }
}
