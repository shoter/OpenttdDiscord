using Discord;
using OpenttdDiscord.Backend.Servers;
using OpenttdDiscord.Openttd.Network.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Messaging
{
    public interface IEmbedFactory
    {
        Embed Create(PacketUdpServerResponse message, Server server);
        
    }
}
