using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Database.Ottd.Servers;
using OpenttdDiscord.Database.Statuses;

namespace OpenttdDiscord.Database
{
    internal class OttdContext : DbContext
    {
        public DbSet<OttdServerEntity> Servers { get; set; }

        public DbSet<StatusMonitorEntity> Monitors { get; set; }

        public OttdContext(DbContextOptions<OttdContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            OttdServerEntity.OnModelCreating(modelBuilder);
        }
    }
}
