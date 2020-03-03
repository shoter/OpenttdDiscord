using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Tcp
{
    public interface ITcpPacketReader
    {
        ITcpMessage Read(Packet packet);
        
    }
}
