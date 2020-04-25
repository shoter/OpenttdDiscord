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
        event EventHandler<SubscribedServer> ServerRemoved;


        Task<SubscribedServer> AddServer(ulong guildId, string name, int port, ulong channelId);
        Task RemoveServer(ulong guildId, string name, ulong channelId);


        Task<bool> Exists(ulong guildId, string name, ulong channelId);

        Task<IEnumerable<SubscribedServer>> GetAllServers(ulong guildId);

        Task UpdateServer(ulong serverId, ulong channelId, ulong messageId);
    }

}
