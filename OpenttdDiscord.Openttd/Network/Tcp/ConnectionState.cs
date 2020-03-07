using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public enum ConnectionState
    {
        NotConnected,
        Connecting,
        DownloadingMap,
        Connected,
        Errored
    }
}
