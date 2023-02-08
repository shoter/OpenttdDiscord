namespace OpenttdDiscord.Domain.DiscordRelated;

public record StatusMessage(Guid ServerId, long MessageId, DateTimeOffset LastUpdateTime);
