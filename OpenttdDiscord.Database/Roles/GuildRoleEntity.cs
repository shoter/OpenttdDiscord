using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Database.Roles;

public record GuildRoleEntity(
    ulong GuildId,
    ulong RoleId
)
{
    public int UserLevel { get; set; }

    [ExcludeFromCodeCoverage]
    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GuildRoleEntity>()
            .HasKey(x => new { x.GuildId, x.RoleId });
    }

    public GuildRole ToDomain() => new(
        GuildId,
        RoleId,
        (UserLevel)UserLevel);
}