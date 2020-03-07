using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public class PacketServerClientInfoMessage : ITcpMessage
    {
        public TcpMessageType MessageType => TcpMessageType.PACKET_SERVER_CLIENT_INFO;

        public uint ClientId { get; }
        public byte PlayAs { get; }
        public string ClientName { get; }

        public PacketServerClientInfoMessage(uint clientId, byte playAs, string clientName)
        {
            this.ClientId = clientId;
            this.PlayAs = playAs;
            this.ClientName = clientName;
        }
    }
}
