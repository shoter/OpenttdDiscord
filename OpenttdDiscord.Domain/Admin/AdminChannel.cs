namespace OpenttdDiscord.Domain.Admin;

public record AdminChannel(Guid ServerId, ulong ChannelId, ulong GuildId, string prefix);
