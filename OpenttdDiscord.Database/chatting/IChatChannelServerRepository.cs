using OpenttdDiscord.Database.Servers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Chatting
{
    public interface IChatChannelServerRepository
    {
        Task<List<ChatChannelServer>> GetAll();

        Task<ChatChannelServer> Get(ulong serverId, ulong channelId);

        Task Remove(ulong serverId, ulong channelId);

        Task<ChatChannelServer> Insert(Server server, ulong channelId);

        Task<bool> Exists(ulong serverId, ulong channelId);
    }
}