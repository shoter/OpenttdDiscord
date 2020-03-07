using RandomAnalyzers.RequiredMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public class PacketClientChatMessage : ITcpMessage
    {
        public ChatDestination ChatDestination { get; }

        public uint Destination { get; }

        public string Message { get;  }

        public long Data { get; } = 0; // I did not see any use of this in ottd client. 

        public TcpMessageType MessageType => TcpMessageType.PACKET_CLIENT_CHAT;

        public PacketClientChatMessage(ChatDestination chatDestination, uint destination, string message)
        {
            this.ChatDestination = chatDestination;
            this.Message = message;
            this.Destination = destination;
        }
    }
}
