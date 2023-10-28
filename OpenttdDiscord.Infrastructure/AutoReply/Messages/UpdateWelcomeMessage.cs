namespace OpenttdDiscord.Infrastructure.AutoReply.Messages
{
    public record UpdateWelcomeMessage(
        ulong GuildId,
        Guid ServerId,
        string NewContent) : AutoReplyMessage(
        GuildId,
        ServerId);
}