using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Servers
{
    public interface ISubscribedServerRepository
    {

        Task<bool> Exists(Server server, ulong channelId);

        Task<SubscribedServer> Add(Server server, int port, ulong channelId);

        Task<SubscribedServer> Get(Server server, int port, ulong channelId);

        Task<IEnumerable<SubscribedServer>> GetAll();

        Task UpdateServer(ulong serverId, ulong channelId, ulong messageId);
        
    }
}
