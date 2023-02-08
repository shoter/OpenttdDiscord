namespace OpenttdDiscord.Domain.DiscordRelated;

public record AdminChannel(Guid ServerId, long ChannelId, string Prefix);
