namespace OpenttdDiscord.Backend.Chatting
{
    public class ChatChannelServer 
    {
        public ulong ServerId { get; }
        public ulong ChannelId { get; }
        public string ServerName { get; }

        public ChatChannelServer(ulong serverId, ulong channelId, string serverName)
        {
            this.ServerId = serverId;
            this.ChannelId = channelId;
            this.ServerName = serverName;
        }
    }
}