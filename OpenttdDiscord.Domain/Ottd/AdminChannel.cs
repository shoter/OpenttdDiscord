namespace OpenttdDiscord.Domain.Ottd;

public record AdminChannel(Guid ServerId, long ChannelId, string Prefix);
