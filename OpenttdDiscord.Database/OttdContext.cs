using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Database.Admin;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Database.Ottd.Servers;
using OpenttdDiscord.Database.Statuses;

namespace OpenttdDiscord.Database
{
    internal class OttdContext : DbContext
    {
        public DbSet<OttdServerEntity> Servers { get; set; } = default!;

        public DbSet<StatusMonitorEntity> Monitors { get; set; } = default!;

        public DbSet<ChatChannelEntity> ChatChannels { get; set; } = default!;

        public DbSet<AdminChannelEntity> AdminChannels { get; set; } = default!;

        public OttdContext(DbContextOptions<OttdContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            OttdServerEntity.OnModelCreating(modelBuilder);
            StatusMonitorEntity.OnModelCreating(modelBuilder);
            ChatChannelEntity.OnModelCreating(modelBuilder);
            AdminChannelEntity.OnModelCreating(modelBuilder);
        }
    }
}
