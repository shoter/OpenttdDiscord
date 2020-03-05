using OpenttdDiscord.Openttd;
using OpenttdDiscord.Openttd.Network.Tcp;
using OpenttdDiscord.Openttd.Network.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Network.Openttd
{
    public class OttdClientFactory : IOttdClientFactory
    {
        private readonly IRevisionTranslator revisionTranslator;

        public OttdClientFactory(IRevisionTranslator revisionTranslator)
        {
            this.revisionTranslator = revisionTranslator;
        }

        public IOttdClient Create(ServerInfo serverInfo, ITcpOttdClient tcpClient, IUdpOttdClient udpClient) => new OttdClient(serverInfo, tcpClient, udpClient, revisionTranslator);
    }
}
