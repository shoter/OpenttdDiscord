namespace OpenttdDiscord.Infrastructure.AutoReplies.Messages
{
    public record UpdateWelcomeMessage(
        ulong GuildId,
        Guid ServerId,
        string Content) : AutoReplyMessage(
        GuildId,
        ServerId);
}