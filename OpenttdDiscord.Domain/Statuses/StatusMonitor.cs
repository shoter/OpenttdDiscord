namespace OpenttdDiscord.Domain.Statuses
{
    public record StatusMonitor(Guid ServerId, ulong GuildId, ulong ChannelId, ulong MessageId, DateTime LastUpdateTime);
}
