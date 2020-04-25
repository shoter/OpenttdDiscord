using OpenttdDiscord.Database.Chatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Chatting
{
    public interface IChatChannelServerService
    {
        event EventHandler<ChatChannelServer> Added;
        event EventHandler<ChatChannelServer> Removed;


        Task<ChatChannelServer> Insert(ulong guildId, string serverName, ulong channelId);
        Task Remove(ulong guildId, string serverName, ulong channelId);
        Task<List<ChatChannelServer>> GetAll(ulong guildId);
        Task<List<ChatChannelServer>> GetAll();

        Task<bool> Exists(ulong guildId, string serverName, ulong channelId);
    }
}
