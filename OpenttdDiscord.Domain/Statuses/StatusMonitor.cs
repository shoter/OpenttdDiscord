namespace OpenttdDiscord.Domain.Statuses
{
    public record StatusMonitor(Guid ServerId, ulong ChannelId, ulong MessageId, DateTimeOffset LastUpdateTime);
}
