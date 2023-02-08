namespace OpenttdDiscord.Domain.Ottd;

public record OttdServer(
    Guid Id,
    string Ip,
    string Name,
    int? PublicPort,
    int? AdminPort,
    string AdminPortPassword
    );
