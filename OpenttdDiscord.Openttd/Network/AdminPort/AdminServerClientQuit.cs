using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminServerClientQuit : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_QUIT;
        
        public uint ClientId { get; }

        public AdminServerClientQuit(uint clientId) => this.ClientId = clientId;
    }
}
