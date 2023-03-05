namespace OpenttdDiscord.Infrastructure.Rcon.Messages
{
    internal record UnregisterRconChannel(Guid ServerId, ulong guildId, ulong channelId);
}
