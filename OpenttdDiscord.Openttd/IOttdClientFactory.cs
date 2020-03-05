using OpenttdDiscord.Openttd.Tcp;
using OpenttdDiscord.Openttd.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd
{
    public interface IOttdClientFactory
    {
        IOttdClient Create(ServerInfo serverInfo, ITcpOttdClient tcpClient, IUdpOttdClient udpClient);
    }
}
