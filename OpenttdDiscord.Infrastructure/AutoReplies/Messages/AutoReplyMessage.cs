using OpenttdDiscord.Infrastructure.Guilds.Messages;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.AutoReplies.Messages
{
    public record AutoReplyMessage(
        ulong GuildId,
        Guid ServerId) : IGuildMessage, IOttdServerMessage;
}