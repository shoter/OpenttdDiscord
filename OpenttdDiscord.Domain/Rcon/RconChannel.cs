namespace OpenttdDiscord.Domain.Rcon;

public record RconChannel(Guid ServerId, ulong ChannelId, ulong GuildId, string prefix);
