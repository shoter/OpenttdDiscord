using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Database.Ottd.Servers;

public record OttdServerEntity(
Guid Id,
long GuildId,
string Ip,
string Name,
int? PublicPort,
int? AdminPort,
string AdminPortPassword
)
{

    public OttdServerEntity(OttdServer ottdServer) : this(
        ottdServer.Id,
        ottdServer.GuildId,
        ottdServer.Ip,
        ottdServer.Name,
        ottdServer.PublicPort,
        ottdServer.AdminPort,
        ottdServer.AdminPortPassword)
    {
    }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OttdServerEntity>()
            .HasKey(x => x.Id);
    }
}
