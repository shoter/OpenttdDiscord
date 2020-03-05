using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Udp
{
    public class PacketUdpClientFindServer : IUdpMessage
    {
        public UdpMessageType MessageType => UdpMessageType.PACKET_UDP_CLIENT_FIND_SERVER;
    }
}
