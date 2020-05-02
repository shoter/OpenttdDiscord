using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public enum AdminEventType
    {
        ChatMessageReceived = 1,
        ConsoleMessage = 2,
        AdminRcon = 3,
        
    }
}
