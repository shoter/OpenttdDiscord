namespace OpenttdDiscord.Domain.Servers;

public record OttdServer(
    Guid Id,
    ulong GuildId,
    string Ip,
    string Name,
    int AdminPort,
    string AdminPortPassword
    );
