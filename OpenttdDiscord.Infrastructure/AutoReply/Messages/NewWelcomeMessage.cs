namespace OpenttdDiscord.Infrastructure.AutoReply.Messages
{
    public record NewWelcomeMessage(
        ulong GuildId,
        Guid ServerId,
        string Content) : AutoReplyMessage(
        GuildId,
        ServerId);
}