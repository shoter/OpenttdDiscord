using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Statuses.Messages
{
    public record RemoveStatusMonitor(
        Guid ServerId,
        ulong GuildId,
        ulong ChannelId) : IOttdServerMessage;
}
