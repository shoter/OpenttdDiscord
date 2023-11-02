using System.Diagnostics.CodeAnalysis;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Database.Ottd.Servers;
using OpenttdDiscord.Domain.AutoReplies;

namespace OpenttdDiscord.Database.AutoReplies
{
    [ExcludeFromCodeCoverage]
    public record WelcomeMessageEntity(
        ulong GuildId,
        Guid ServerId
    )
    {
        public string Content { get; set; } = string.Empty;

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

        internal EitherAsyncUnit Update(string content)
        {
            this.Content = content;
            return Unit.Default;
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