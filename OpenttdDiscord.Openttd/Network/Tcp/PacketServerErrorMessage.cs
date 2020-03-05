using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Tcp
{
    public class PacketServerErrorMessage : ITcpMessage
    {
        public TcpMessageType MessageType => TcpMessageType.PACKET_SERVER_ERROR;

        public byte ErrorCode { get; }
        public PacketServerErrorMessage(byte errorCode)
        {
            this.ErrorCode = errorCode;
        }
    }
}
