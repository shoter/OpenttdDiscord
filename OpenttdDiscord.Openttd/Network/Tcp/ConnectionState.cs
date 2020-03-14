using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public enum ConnectionState
    {
        Idle,
        NotConnected,
        Connecting,
        DownloadingMap,
        Connected,
        Errored
    }
}
