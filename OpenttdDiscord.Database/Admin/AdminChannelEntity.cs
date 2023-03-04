using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Domain.Admin;

namespace OpenttdDiscord.Database.Admin
{
    internal class AdminChannelEntity
    {
        public Guid ServerId { get; internal set; }

        public ulong GuildId { get; internal set; }

        public ulong ChannelId { get; internal set; }

        public string Prefix { get; internal set; }

        public AdminChannelEntity()
        {
            Prefix = string.Empty;
        }

        public AdminChannelEntity(AdminChannel ac)
        {
            ServerId = ac.ServerId;
            GuildId = ac.GuildId;
            ChannelId = ac.ChannelId;
            Prefix = ac.prefix;
        }

        public AdminChannel ToDomain() => new(
            ServerId,
            GuildId,
            ChannelId,
            Prefix);

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminChannelEntity>()
                .HasKey(x => new { x.ServerId, x.ChannelId });
        }
    }
}
