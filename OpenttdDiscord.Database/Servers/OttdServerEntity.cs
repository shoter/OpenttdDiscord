using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Database.Ottd.Servers;

public record OttdServerEntity(
Guid Id,
ulong GuildId,
string Ip,
string Name,
int AdminPort,
string AdminPortPassword
)
{

    public OttdServerEntity(OttdServer ottdServer) : this(
        ottdServer.Id,
        ottdServer.GuildId,
        ottdServer.Ip,
        ottdServer.Name,
        ottdServer.AdminPort,
        ottdServer.AdminPortPassword)
    {
    }

    public OttdServer ToOttdServer()
    {
        return new(
            Id,
            GuildId,
            Ip,
            Name,
            AdminPort,
            AdminPortPassword);
    }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OttdServerEntity>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<OttdServerEntity>()
            .HasIndex(x => x.Name)
            .IsUnique(true);
    }
}
