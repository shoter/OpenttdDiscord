using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    /// <summary>
    /// A server tells that a client has quit.
    /// </summary>
    public class PacketServerQuitMessage : ITcpMessage
    {
        public TcpMessageType MessageType => TcpMessageType.PACKET_SERVER_QUIT;

        public uint ClientId { get; }

        public PacketServerQuitMessage(uint clientId)
        {
            this.ClientId = clientId;
        }
    }
}
