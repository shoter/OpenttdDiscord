using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Servers
{
    public interface ISubscribedServerRepository
    {

        Task<bool> Exists(string ip, int port);

        Task<SubscribedServer> Add(Server server, ulong channelId);

        Task UpdateServer(ulong serverId, ulong messageId);
        
    }
}
