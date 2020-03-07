using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public class GenericTcpMessage : ITcpMessage
    {
        public TcpMessageType MessageType { get; }

        public GenericTcpMessage(TcpMessageType type) => MessageType = type;
    }
}
