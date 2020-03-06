using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Udp
{
    public class UdpOttdClientFactory : IUdpOttdClientFactory
    {
        private readonly IUdpPacketService udpPacketService;

        public UdpOttdClientFactory(IUdpPacketService udpPacketService)
        {
            this.udpPacketService = udpPacketService;
        }

        public IUdpOttdClient Create() => new UdpOttdClient(udpPacketService);
    }
}
