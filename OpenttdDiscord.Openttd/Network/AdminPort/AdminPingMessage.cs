using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminPingMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_ADMIN_PING;

        public uint Argument { get; set; }

        public AdminPingMessage(uint argument = 0)
        {
            Argument = argument;
        }
    }
}
