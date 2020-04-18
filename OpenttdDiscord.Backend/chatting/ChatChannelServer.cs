using OpenttdDiscord.Backend.Servers;

namespace OpenttdDiscord.Backend.Chatting
{
    public class ChatChannelServer 
    {
        public Server Server { get; internal set; }
        public ulong ChannelId { get; internal set; }
        public bool JoinMessagesEnabled { get; internal set; }
    }
}