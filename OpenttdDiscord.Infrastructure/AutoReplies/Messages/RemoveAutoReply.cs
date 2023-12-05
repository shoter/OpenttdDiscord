namespace OpenttdDiscord.Infrastructure.AutoReplies.Messages
{
    public record RemoveAutoReply(
        ulong GuildId,
        Guid ServerId,
        string TriggerMessage) : AutoReplyMessage(
        GuildId,
        ServerId);
}