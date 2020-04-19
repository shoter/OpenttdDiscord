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

        Task<ChatChannelServer> Insert(string serverName, ulong channelId);
        Task<List<ChatChannelServer>> GetAll();
        Task<bool> Exists(string serverName, ulong channelId);
    }
}
