namespace OpenttdDiscord.Backend.Chatting
{
    public class ChatChannelServer 
    {
        public ulong ServerId { get; internal set; }
        public ulong ChannelId { get; internal set; }
        public string ServerName { get; internal set; }
        public bool JoinMessagesEnabled { get; internal set; }
    }
}