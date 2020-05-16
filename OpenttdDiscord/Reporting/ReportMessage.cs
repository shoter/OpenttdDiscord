using OpenttdDiscord.Database.Servers;
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

        public Server Server { get; }

        public DateTime ReportTime { get; } = DateTime.Now;
        
        public string Reason { get; }

        public string ReporterName { get; }

        public ReportMessage(Server server, string reporterName, string reason)
        {
            this.Server = server;
            this.ReporterName = reporterName;
            this.Reason = reason;
        }

        public void AddSection(ReportSection s) => Sections.Add(s);
    }
}
