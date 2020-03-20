using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminPollMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_ADMIN_POLL;

        public AdminUpdateType UpdateType { get; }

        public uint Argument { get; }

        public AdminPollMessage(AdminUpdateType updateType, uint argument)
        {
            this.UpdateType = updateType;
            this.Argument = argument;
        }
    }
}
