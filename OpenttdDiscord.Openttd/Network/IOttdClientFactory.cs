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
    public interface IOttdClientFactory
    {
        IOttdClient Create(ServerInfo serverInfo);
    }
}
