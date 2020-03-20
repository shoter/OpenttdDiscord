using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminJoinMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_ADMIN_JOIN;

        public string Password { get; }

        public string AdminName { get; }

        public string AdminVersion { get; }

        public AdminJoinMessage(string password, string adminName, string adminVersion)
        {
            this.Password = password;
            this.AdminName = adminName;
            this.AdminVersion = adminVersion;
        }
    }
}
