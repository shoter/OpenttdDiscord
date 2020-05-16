using OpenttdDiscord.Database.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Reporting
{
    public class ReportServerInfo
    {
        public ReportServer ReportServer { get; }

        public Queue<string> LastMessages { get; } = new Queue<string>();

        public ReportServerState ServerState { get; set; } = ReportServerState.Listening;

        public ReportMessage Report { get; set; } = null;

        public ReportSection CurrentNewSection { get; set; } = null;

        public uint CurrentPingValue { get; set; }


        public void AddMessage(string message)
        {
            if(LastMessages.Count > 200)
                LastMessages.Dequeue();
            LastMessages.Enqueue(message);
        }

        public ReportServerInfo(ReportServer server)
        {
            this.ReportServer = server;
        }        
    }
}
