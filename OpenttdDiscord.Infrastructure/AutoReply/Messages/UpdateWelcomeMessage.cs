namespace OpenttdDiscord.Infrastructure.AutoReply.Messages
{
    public record UpdateWelcomeMessage(
        ulong GuildId,
        Guid ServerId,
        string Content) : AutoReplyMessage(
        GuildId,
        ServerId);
}