using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Domain.Rcon;

namespace OpenttdDiscord.Database.Rcon
{
    internal class RconChannelEntity
    {
        public Guid ServerId { get; internal set; }

        public ulong GuildId { get; internal set; }

        public ulong ChannelId { get; internal set; }

        public string Prefix { get; internal set; }

        public RconChannelEntity()
        {
            Prefix = string.Empty;
        }

        public RconChannelEntity(RconChannel ac)
        {
            ServerId = ac.ServerId;
            GuildId = ac.GuildId;
            ChannelId = ac.ChannelId;
            Prefix = ac.Prefix;
        }

        public RconChannel ToDomain() => new(
            ServerId,
            GuildId,
            ChannelId,
            Prefix);

        [ExcludeFromCodeCoverage]
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RconChannelEntity>()
                .HasKey(
                    x => new
                    {
                        x.ServerId,
                        x.ChannelId,
                    });
        }
    }
}