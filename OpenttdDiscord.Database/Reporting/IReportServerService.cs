using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Reporting
{
    public interface IReportServerService
    {
        event EventHandler<ReportServer> Added;
        event EventHandler<ReportServer> Removed;

        Task<ReportServer> Add(Server server, ulong channelId);

        Task<List<ReportServer>> GetAllForGuild(ulong guildId);

        Task Remove(ReportServer reportServer);
    }
}
