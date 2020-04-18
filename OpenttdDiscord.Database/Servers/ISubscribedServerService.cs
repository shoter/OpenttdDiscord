using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Servers
{
    public interface ISubscribedServerService
    {
        event EventHandler<SubscribedServer> ServerAdded;
        Task<SubscribedServer> AddServer(string ip, int port, ulong channelId);

        Task<bool> Exists(string ip, int port, ulong channelId);

        Task<IEnumerable<SubscribedServer>> GetAllServers();

        Task UpdateServer(ulong serverId, ulong channelId, ulong messageId);
    }

}
