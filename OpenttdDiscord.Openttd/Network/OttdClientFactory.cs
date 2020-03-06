using OpenttdDiscord.Openttd;
using OpenttdDiscord.Openttd.Network;
using OpenttdDiscord.Openttd.Network.Tcp;
using OpenttdDiscord.Openttd.Network.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network
{
    public class OttdClientFactory : IOttdClientFactory
    {
        private readonly IRevisionTranslator revisionTranslator;
        private readonly ITcpOttdClientFactory tcpClientFactory;
        private readonly IUdpOttdClientFactory udpClientFactory;

        public OttdClientFactory(IRevisionTranslator revisionTranslator, ITcpOttdClientFactory tcpClientFactory, IUdpOttdClientFactory udpClientFactory)
        {
            this.revisionTranslator = revisionTranslator;
            this.tcpClientFactory = tcpClientFactory;
            this.udpClientFactory = udpClientFactory;
        }

        public IOttdClient Create(ServerInfo serverInfo) => new OttdClient(serverInfo, tcpClientFactory.Create(), udpClientFactory.Create(), revisionTranslator);
    }
}
