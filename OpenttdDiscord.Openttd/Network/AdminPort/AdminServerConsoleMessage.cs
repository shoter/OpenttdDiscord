using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminServerConsoleMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CONSOLE;

        public string Origin { get; }

        public string Message { get; }

        public AdminServerConsoleMessage(string origin, string message)
        {
            this.Origin = origin;
            this.Message = message;
        }
    }
}
