using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Servers
{
    public interface ISubscribedServerService
    {
        Task<SubscribedServer> AddServer(string ip, int port, ulong channelId);

        Task UpdateServer(ulong serverId, ulong messageId);

    }
}
