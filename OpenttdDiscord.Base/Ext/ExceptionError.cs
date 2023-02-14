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
    }
}
