using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminServerClientUpdateMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_UPDATE;

        public uint ClientId { get; }

        public string ClientName { get; }

        public byte PlayingAs { get; }

        public AdminServerClientUpdateMessage(uint clientId, string clientName, byte playingAs)
        {
            this.ClientId = clientId;
            this.ClientName = clientName;
            this.PlayingAs = playingAs;
        }
    }
}
