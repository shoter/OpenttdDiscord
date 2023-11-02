using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Database.AutoReplies;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Database.Ottd.Servers;
using OpenttdDiscord.Database.Rcon;
using OpenttdDiscord.Database.Reporting;
using OpenttdDiscord.Database.Roles;
using OpenttdDiscord.Database.Statuses;

namespace OpenttdDiscord.Database
{
    [ExcludeFromCodeCoverage]
    internal class OttdContext : DbContext
    {
        public DbSet<OttdServerEntity> Servers { get; set; } = default!;
        public DbSet<StatusMonitorEntity> Monitors { get; set; } = default!;
        public DbSet<ChatChannelEntity> ChatChannels { get; set; } = default!;
        public DbSet<RconChannelEntity> RconChannels { get; set; } = default!;
        public DbSet<ReportChannelEntity> ReportChannels { get; set; } = default!;
        public DbSet<GuildRoleEntity> GuildRoles { get; set; } = default!;
        public DbSet<WelcomeMessageEntity> WelcomeMessages { get; set; } = default!;
        public DbSet<AutoReplyEntity> AutoReplies { get; set; } = default!;


        public OttdContext(DbContextOptions<OttdContext> options)
            : base(options)
        {
        }

        [ExcludeFromCodeCoverage]
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            OttdServerEntity.OnModelCreating(modelBuilder);
            StatusMonitorEntity.OnModelCreating(modelBuilder);
            ChatChannelEntity.OnModelCreating(modelBuilder);
            RconChannelEntity.OnModelCreating(modelBuilder);
            ReportChannelEntity.OnModelCreating(modelBuilder);
            GuildRoleEntity.OnModelCreating(modelBuilder);
            WelcomeMessageEntity.OnModelCreating(modelBuilder);
            AutoReplyEntity.OnModelCreating(modelBuilder);
        }
    }
}
