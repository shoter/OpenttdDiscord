using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public class PacketServerJoinMessage : ITcpMessage
    {
        public TcpMessageType MessageType => TcpMessageType.PACKET_SERVER_JOIN;

        public uint ClientId { get; }

        public PacketServerJoinMessage(uint clientId)
        {
            this.ClientId = clientId;
        }
    }
}
