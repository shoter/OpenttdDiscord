using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Tcp
{
    public class PacketServerErrorQuitMessage : ITcpMessage
    {
        public TcpMessageType MessageType => TcpMessageType.PACKET_SERVER_ERROR_QUIT;

        public uint ClientId { get; }

        public PacketServerErrorQuitMessage(uint clientId) => this.ClientId = clientId;
    }
}
