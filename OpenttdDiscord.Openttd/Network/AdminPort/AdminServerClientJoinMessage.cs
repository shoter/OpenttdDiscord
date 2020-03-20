using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminServerClientJoinMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_JOIN;

        public uint ClientId { get; }

        public AdminServerClientJoinMessage(uint clientId) => ClientId = clientId;
    }
}
