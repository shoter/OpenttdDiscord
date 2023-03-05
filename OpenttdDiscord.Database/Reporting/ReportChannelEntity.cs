using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Domain.Reporting;

namespace OpenttdDiscord.Database.Reporting
{
    internal class ReportChannelEntity
    {
        public Guid ServerId { get; set; }

        public ulong GuildId { get; set; }

        public ulong ChannelId { get; set; }

        public ReportChannelEntity()
        {
        }

        public ReportChannelEntity(ReportChannel rc)
        {
            ServerId = rc.ServerId;
            GuildId = rc.GuildId;
            ChannelId = rc.ChannelId;
        }

        public ReportChannel ToDomain() => new(
            ServerId,
            GuildId,
            ChannelId);

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReportChannelEntity>()
                .HasKey(x => new { x.ServerId, x.ChannelId });
        }
    }
}
