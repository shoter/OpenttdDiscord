using OpenttdDiscord.Openttd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Reporting
{
    public class ReportMessage
    {
        public List<ReportSection> Sections { get; } = new List<ReportSection>();

        public ServerInfo ServerInfo { get; }

        public DateTime ReportTime { get; } = DateTime.Now;

        public string ReporterName { get; }

        public string ReporterIp { get; }

        public ReportMessage(ServerInfo serverInfo, string reporterName, string reporterIp)
        {
            this.ServerInfo = serverInfo;
            this.ReporterIp = reporterIp;
            this.ReporterName = reporterName;
        }

        public void AddSection(ReportSection s) => Sections.Add(s);
    }
}
