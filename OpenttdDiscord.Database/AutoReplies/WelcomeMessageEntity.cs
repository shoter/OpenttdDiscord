using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Database.Ottd.Servers;

namespace OpenttdDiscord.Database.AutoReplies
{
    public record WelcomeMessageEntity(
        ulong GuildId,
        Guid ServerId,
        string Content
    )
    {
        [ExcludeFromCodeCoverage]
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WelcomeMessageEntity>()
                .HasKey(x => x.GuildId);

            modelBuilder.Entity<WelcomeMessageEntity>()
                .HasOne<OttdServerEntity>()
                .WithMany()
                .HasForeignKey(x => x.ServerId);
        }
    }
}
