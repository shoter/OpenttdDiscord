using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network
{
    public class ReceivedChatMessage
    {
        public ServerInfo ServerInfo { get; }
        public Player Player { get; }

        public string Message { get; }

        public ChatDestination ChatDestination { get; }

        public ReceivedChatMessage(string msg, Player player, ChatDestination chatDestination, ServerInfo serverInfo)
        {
            this.Player = player;
            this.Message = msg;
            this.ChatDestination = chatDestination;
            this.ServerInfo = serverInfo;
        }


    }
}
