using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Database.Ottd.Servers;
using OpenttdDiscord.Domain.Statuses;

namespace OpenttdDiscord.Database.Statuses;

public class StatusMonitorEntity
{
    public Guid ServerId { get; set; }

    public ulong GuildId { get; set; }

    public ulong ChannelId { get; set; }

    public ulong MessageId { get; set; }

    public DateTime LastUpdateTime { get; set; }

    public OttdServerEntity Server { get; set; } = default!;

    public StatusMonitorEntity()
    {
    }

    public StatusMonitorEntity(StatusMonitor sm)
    {
        ServerId = sm.ServerId;
        GuildId = sm.GuildId;
        ChannelId = sm.ChannelId;
        MessageId = sm.MessageId;
        LastUpdateTime = sm.LastUpdateTime.ToUniversalTime();
    }

    public StatusMonitor ToDomain()
    {
        return new(
            ServerId,
            GuildId,
            ChannelId,
            MessageId,
            LastUpdateTime.ToUniversalTime()
            );
    }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StatusMonitorEntity>()
            .HasKey(x => new { x.ServerId, x.ChannelId });

        modelBuilder.Entity<StatusMonitorEntity>()
            .HasOne(x => x.Server)
            .WithMany(s => s.Monitors)
            .HasForeignKey(x => x.ServerId);
    }
}
