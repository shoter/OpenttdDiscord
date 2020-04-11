using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminServerInfo
    {
        public string ServerName { get; internal set; }

        public string RevisionName { get; internal set; }

        public bool IsDedicated { get; internal set; }

        public string MapName { get; internal set; }
        
    }
}
