namespace OpenttdDiscord.Infrastructure.AutoReplies.Messages
{
    /// <summary>
    /// Indicates that for given `AdminChatMessageEvent` there was no response from any of auto reply instance actors
    /// </summary>
    public record NoResponseForMessage
    {
        public static NoResponseForMessage Instance { get; } = new();

        private NoResponseForMessage()
        {
        }
    }
}