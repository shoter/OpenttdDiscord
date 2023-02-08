using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Database.Ottd.Servers;

namespace OpenttdDiscord.Database
{
    internal class OttdContext : DbContext
    {
        public DbSet<OttdServerEntity> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            OttdServerEntity.OnModelCreating(modelBuilder);
        }
    }
}
