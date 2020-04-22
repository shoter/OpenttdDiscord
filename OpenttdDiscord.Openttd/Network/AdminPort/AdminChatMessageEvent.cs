using OpenttdDiscord.Common;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminChatMessageEvent : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.ChatMessageReceived;


        public Player Player { get; }
        public string Message { get; }

        public ServerInfo Server { get; }
        public AdminChatMessageEvent(Player player, string msg, ServerInfo info)
        {
            this.Player = player;
            this.Message = msg;
            this.Server = info;
        }
    }
}
