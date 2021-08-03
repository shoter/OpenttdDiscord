using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenttdDiscord.Database.Servers;

namespace OpenttdDiscord.Database.AntiGrief
{
    public interface IAntiGriefRepository
    {
        Task<AntiGriefServer> Add(Server server, ulong channelId);
        Task<List<AntiGriefServer>> GetAll(ulong channelId);
        Task<AntiGriefServer> Get(ulong serverId, ulong channelId);
        Task<List<AntiGriefServer>> GetAll();
        Task<List<AntiGriefServer>> GetAllForGuild(ulong guildId);
        Task Remove(AntiGriefServer reportServer);

    }
}
