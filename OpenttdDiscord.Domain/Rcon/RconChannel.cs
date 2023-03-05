namespace OpenttdDiscord.Domain.Rcon;

public record RconChannel(Guid ServerId, ulong GuildId, ulong ChannelId, string Prefix);
