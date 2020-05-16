using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminPongEvent : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.Pong;

        public ServerInfo Server { get; }

        public uint PongValue { get; }

        public AdminPongEvent(ServerInfo server, uint pongValue)
        {
            this.Server = server;
            this.PongValue = pongValue;
        }
    }
}
