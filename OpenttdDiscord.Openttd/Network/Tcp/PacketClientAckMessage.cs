using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public class PacketClientAckMessage : ITcpMessage
    {
        public TcpMessageType MessageType => TcpMessageType.PACKET_CLIENT_ACK;

        public uint FrameCounter { get; }
        public byte Token { get; }

        public PacketClientAckMessage(uint frameCounter, byte token)
        {
            this.Token = token;
            this.FrameCounter = frameCounter;
        }
    }
}
