namespace OpenttdDiscord.Base.Ext
{
    public class HumanReadableError : IError
    {
        public string Reason { get; }

        public HumanReadableError(string reason)
        {
            this.Reason = reason;
        }
    }
}
