using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenttdDiscord.Database.Servers;

namespace OpenttdDiscord.Database.AntiGrief
{
    public interface IAntiGriefService
    {
        event EventHandler<AntiGriefServer> Added;
        event EventHandler<AntiGriefServer> Removed;

        Task<AntiGriefServer> Add(Server server);

        Task<List<AntiGriefServer>> GetAllForGuild(ulong guildId);

        Task<List<AntiGriefServer>> GetAll();

        Task<AntiGriefServer> Get(ulong serverId);

        Task Remove(AntiGriefServer reportServer);


    }
}
