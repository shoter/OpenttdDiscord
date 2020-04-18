using OpenttdDiscord.Backend.Servers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Chatting
{
    public interface IChatChannelServerRepository
    {
        Task<List<ChatChannelServer>> GetAllAsync();

        Task<ChatChannelServer> Get(ulong serverId, ulong channelId);

        Task<ChatChannelServer> Insert(Server server, ulong channelId);

        Task<bool> Exists(ulong serverId, ulong channelId);
    }
}