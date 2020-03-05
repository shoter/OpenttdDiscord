using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Tcp
{
    public class PacketServerWelcomeMessage : ITcpMessage
    {
        public TcpMessageType MessageType => TcpMessageType.PACKET_SERVER_WELCOME;

        public uint ClientId { get; }

        public uint GenerationSeed { get; }

        public string NetworkId { get; }

        public PacketServerWelcomeMessage(uint clientId, uint generationSeed, string networkId)
        {
            this.ClientId = clientId;
            this.GenerationSeed = generationSeed;
            this.NetworkId = networkId;
        }

    }
}
