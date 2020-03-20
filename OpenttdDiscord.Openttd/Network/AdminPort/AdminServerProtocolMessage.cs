using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminServerProtocolMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_PROTOCOL;

        public Dictionary<AdminUpdateType, ushort> AdminUpdateSettings;
        public AdminServerProtocolMessage(Dictionary<AdminUpdateType, ushort> adminUpdateSettings)
        {
            this.AdminUpdateSettings = adminUpdateSettings;
        }
    }
}
