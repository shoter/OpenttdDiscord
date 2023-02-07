namespace OpenttdDiscord.Domain.Ottd;

public record OttdServer(
    string Ip,
    string Name,
    int? PublicPort,
    int? AdminPort
    );
