using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public interface IAdminEvent
    {
        AdminEventType EventType { get; }

        ServerInfo Server { get; }
    }
}
