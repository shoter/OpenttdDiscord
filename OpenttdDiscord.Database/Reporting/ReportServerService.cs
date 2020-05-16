using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Reporting
{
    public class ReportServerService : IReportServerService
    {
        public event EventHandler<ReportServer> Added;
        public event EventHandler<ReportServer> Removed;

        private readonly IReportServerRepository reportServerRepository;

        public ReportServerService(IReportServerRepository reportServerRepository)
        {
            this.reportServerRepository = reportServerRepository;
        }

        public async Task<ReportServer> Add(Server server, ulong channelId)
        {
            ReportServer reportServer = await reportServerRepository.Add(server, channelId);
            this.Added?.Invoke(this, reportServer);
            return reportServer;
        }

        public Task<List<ReportServer>> GetAllForGuild(ulong guildId) => this.reportServerRepository.GetAllForGuild(guildId);

        public async Task Remove(ReportServer reportServer)
        {
            await reportServerRepository.Remove(reportServer);
            this.Removed?.Invoke(this, reportServer);
        }

        public Task<ReportServer> Get(ulong serverId, ulong channelId) => this.reportServerRepository.Get(serverId, channelId);

        public Task<List<ReportServer>> GetAll() => this.reportServerRepository.GetAll();
    }
}
