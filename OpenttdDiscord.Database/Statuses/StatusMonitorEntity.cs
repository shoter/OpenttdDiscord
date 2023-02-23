using OpenttdDiscord.Domain.Statuses;

namespace OpenttdDiscord.Database.Statuses;

public class StatusMonitorEntity
{
    internal Guid ServerId { get; set; }

    internal ulong ChannelId { get; set; }

    internal ulong MessageId { get; set; }

    internal DateTime LastUpdateTime { get; set; }

    public StatusMonitorEntity(StatusMonitor sm)
    {
        ServerId = sm.ServerId;
        ChannelId = sm.ChannelId;
        MessageId = sm.MessageId;  
        LastUpdateTime = sm.LastUpdateTime.UtcDateTime;
    }

    public StatusMonitor ToDomain()
    {
        return new(
            ServerId,
            ChannelId,
            MessageId,
            new DateTimeOffset(LastUpdateTime, TimeSpan.Zero)
            );
    }
}
