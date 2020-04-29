using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Admins
{
    public class AdminChannelRepository : IAdminChannelRepository
    {
        public AdminChannelRepository(MySqlConfig config)
        {

        }
        public Task<List<AdminChannel>> GetAdminChannels(Server server)
        {
            throw new NotImplementedException();
        }

        public Task<List<AdminChannel>> GetAdminChannels(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public Task<AdminChannel> Insert(Server server, ulong channelId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveChannel(ulong serverId, ulong channelId)
        {
            throw new NotImplementedException();
        }
    }
}
