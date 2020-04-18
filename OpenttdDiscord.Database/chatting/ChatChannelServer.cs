using OpenttdDiscord.Database.Servers;

namespace OpenttdDiscord.Database.Chatting
{
    public class ChatChannelServer 
    {
        public Server Server { get; internal set; }
        public ulong ChannelId { get; internal set; }
        public bool JoinMessagesEnabled { get; internal set; }
    }
}