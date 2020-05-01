using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Admins
{
    public interface IAdminChannelService
    {
        event EventHandler<AdminChannel> Added;
        event EventHandler<AdminChannel> Removed;

        Task<AdminChannel> Add(Server server, ulong channelId, string prefix);

        Task<List<AdminChannel>> GetAll(ulong guildId);
        Task<List<AdminChannel>> GetAll(Server server);
        Task<List<AdminChannel>> GetAllForChannel(ulong channelId);

        Task Remove(AdminChannel adminChannel);
    }
}
