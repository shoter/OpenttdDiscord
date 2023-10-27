using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Database.Ottd.Servers;
using OpenttdDiscord.Domain.AutoReplies;

namespace OpenttdDiscord.Database.AutoReplies
{
    public record WelcomeMessageEntity(
        ulong GuildId,
        Guid ServerId
    )
    {
        internal string Content { get; set; } = string.Empty;

        public WelcomeMessage ToDomain() => new(
            ServerId,
            Content);

        public WelcomeMessageEntity(
            ulong guildId,
            Guid serverId,
            string content)
            : this(
                guildId,
                serverId)
        {
            this.Content = content;
        }

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