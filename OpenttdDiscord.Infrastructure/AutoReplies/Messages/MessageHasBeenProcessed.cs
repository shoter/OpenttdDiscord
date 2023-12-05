namespace OpenttdDiscord.Infrastructure.AutoReplies.Messages
{
    /// <summary>
    /// Indicates that given `AdminChatMessageEvent` has been successfully processed and some form auto reply was generated.
    /// </summary>
    public record MessageHasBeenProcessed
    {
        public static MessageHasBeenProcessed Instance { get; } = new();

        private MessageHasBeenProcessed()
        {
        }
    }
}