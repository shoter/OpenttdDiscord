using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminRconEvent : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.AdminRcon;

        public ServerInfo Server { get; } 

        public string Message { get; }

        public AdminRconEvent(ServerInfo server, string msg)
        {
            this.Server = server;
            this.Message = msg;
        }
    }
}
