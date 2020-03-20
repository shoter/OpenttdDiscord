using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminServerClientErrorMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_ERROR;

        public uint ClientId { get; }

        public NetworkErrorCode Error { get; }

        public AdminServerClientErrorMessage(uint clientId, byte error)
        {
            this.ClientId = clientId;
            this.Error = (NetworkErrorCode)error;
        }
    }
}
