using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminConsoleEvent : IAdminEvent
    {
        public string Origin { get; }

        public string Message { get; }

        public AdminEventType EventType => AdminEventType.ConsoleMessage;
        public ServerInfo Server { get; }

        public AdminConsoleEvent(ServerInfo server, string origin, string message)
        {
            this.Origin = origin;
            this.Message = message;
            this.Server = server;
        }
        
    }
}
