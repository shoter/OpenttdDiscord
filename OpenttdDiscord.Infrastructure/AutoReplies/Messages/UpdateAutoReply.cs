using OpenttdDiscord.Domain.AutoReplies;

namespace OpenttdDiscord.Infrastructure.AutoReplies.Messages
{
    public record UpdateAutoReply(
        ulong GuildId,
        Guid ServerId,
        AutoReply AutoReply) : AutoReplyMessage(
        GuildId,
        ServerId);
}