namespace OpenttdDiscord.Domain.Chatting
{
    public record ChatChannel(Guid ServerId, ulong GuildId, ulong ChannelId);
}
