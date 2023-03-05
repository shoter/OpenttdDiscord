namespace OpenttdDiscord.Infrastructure.Rcon.Messages
{
    internal record UnregisterRconChannel(Guid serverId, ulong guildId, ulong channelId);
}
