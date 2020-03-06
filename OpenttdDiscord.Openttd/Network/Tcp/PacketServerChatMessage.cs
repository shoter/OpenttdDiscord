using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public class PacketServerChatMessage : ITcpMessage
    {
        public NetworkAction NetworkAction { get; }

        public uint ClientId { get; }

        public bool SelfSend { get; }

        public string Message { get; }

        public long Data { get; }

        public TcpMessageType MessageType => TcpMessageType.PACKET_SERVER_CHAT;

        public PacketServerChatMessage(NetworkAction action, uint clientId, bool selfSend, string message, long data)
        {
            this.NetworkAction = action;
            this.ClientId = clientId;
            this.Message = message;
            this.SelfSend = selfSend;
            this.Data = data;
        }
    }
}
