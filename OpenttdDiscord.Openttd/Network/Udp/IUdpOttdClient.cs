using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Udp
{
    public interface IUdpOttdClient
    {
        Task<IUdpMessage> SendMessage(IUdpMessage message, string ip, int port);

    }
}
