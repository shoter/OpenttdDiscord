using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Udp
{
    public interface IUdpPacketService
    {
        Packet CreatePacket(IUdpMessage message);

        IUdpMessage ReadPacket(Packet packet);
        
    }
}
