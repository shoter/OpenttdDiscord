using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class GenericAdminMessage : IAdminMessage
    {
        public AdminMessageType MessageType { get; }

        public GenericAdminMessage(AdminMessageType type) => MessageType = type;
    }
}
