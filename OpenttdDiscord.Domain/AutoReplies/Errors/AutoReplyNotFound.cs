namespace OpenttdDiscord.Domain.AutoReplies.Errors
{
    public class AutoReplyNotFound : HumanReadableError
    {
        public AutoReplyNotFound()
        : base("Auto reply not found")
        {
        }

        public static AutoReplyNotFound Instance { get; } = new AutoReplyNotFound();
    }
}
