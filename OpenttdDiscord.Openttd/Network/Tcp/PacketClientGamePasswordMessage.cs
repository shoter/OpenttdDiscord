using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public class PacketClientGamePasswordMessage : ITcpMessage
    {
        public TcpMessageType MessageType => TcpMessageType.PACKET_CLIENT_GAME_PASSWORD;

        public string Password { get; }

        public PacketClientGamePasswordMessage(string password)
        {
            this.Password = password;
        }
    }
}
