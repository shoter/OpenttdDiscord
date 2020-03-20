using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminPingMessage : IAdminMessage
    {
        public AdminMessageType MessageType => throw new NotImplementedException();

        public uint Argument { get; set; }
    }
}
