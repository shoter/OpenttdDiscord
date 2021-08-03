using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using OpenttdDiscord.Database.Servers;

namespace OpenttdDiscord.Database.AntiGrief
{
    public class AntiGriefService : IAntiGriefService
    {
        public event EventHandler<AntiGriefServer> Added;
        public event EventHandler<AntiGriefServer> Removed;

        private readonly IAntiGriefRepository antiGriefRepository;

        public AntiGriefService(IAntiGriefRepository antiGriefRepository)
        {
            this.antiGriefRepository = antiGriefRepository;
        }

        public async Task<AntiGriefServer> Add(Server server, TimeSpan requiredTimeToPlay, string reason)
        {
            AntiGriefServer reportServer = await antiGriefRepository.Add(server, requiredTimeToPlay, reason);
            this.Added?.Invoke(this, reportServer);
            return reportServer;
        }

        public Task<List<AntiGriefServer>> GetAllForGuild(ulong guildId) => this.antiGriefRepository.GetAllForGuild(guildId);

        public async Task Remove(AntiGriefServer reportServer)
        {
            await antiGriefRepository.Remove(reportServer);
            this.Removed?.Invoke(this, reportServer);
        }

        public Task<AntiGriefServer> Get(ulong serverId) => this.antiGriefRepository.Get(serverId);

        public Task<List<AntiGriefServer>> GetAll() => this.antiGriefRepository.GetAll();

        public async Task<bool> Exists(ulong serverId)
            => await Get(serverId) != null;
    }
}
