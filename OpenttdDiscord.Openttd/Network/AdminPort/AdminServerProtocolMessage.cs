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

        public byte NetworkVersion { get; }

        public Dictionary<AdminUpdateType, UpdateFrequency> AdminUpdateSettings;
        public AdminServerProtocolMessage(byte networkVersion, Dictionary<AdminUpdateType, UpdateFrequency> adminUpdateSettings)
        {
            this.NetworkVersion = networkVersion;
            this.AdminUpdateSettings = adminUpdateSettings;
        }
    }
}
