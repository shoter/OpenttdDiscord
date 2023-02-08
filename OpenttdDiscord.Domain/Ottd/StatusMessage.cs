namespace OpenttdDiscord.Domain.Ottd;

public record StatusMessage(Guid ServerId, long MessageId, DateTimeOffset LastUpdateTime);
