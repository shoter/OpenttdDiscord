using Discord;
using OpenttdDiscord.Backend.Servers;
using OpenttdDiscord.Openttd.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Embeds
{
    public interface IUdpEmbedFactory
    {
        Task<Embed> Create(IUdpMessage message, Server server);
        
    }
}
