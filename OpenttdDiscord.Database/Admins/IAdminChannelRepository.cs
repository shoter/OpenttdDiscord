using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Admins
{
    public interface IAdminChannelRepository
    {
        Task<AdminChannel> Insert(Server server, ulong channelId, string prefix);

        /// <summary>
        /// Gets admin channels for specified server.
        /// </summary>
        Task<List<AdminChannel>> GetAdminChannels(Server server);
        Task<List<AdminChannel>> GetAll();
        Task<AdminChannel> GetAdminChannelsForChannel(ulong channelId);

        Task<AdminChannel> ChangePrefix(AdminChannel adminChannel, string newPrefix);


        Task<List<AdminChannel>> GetAdminChannels(ulong guildId);

        Task RemoveChannel(ulong serverId, ulong channelId);
        
    }
}
