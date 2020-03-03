using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Tcp
{
    public class PacketServerSyncMessage : ITcpMessage
    {
        public TcpMessageType MessageType => TcpMessageType.PACKET_SERVER_SYNC;

        public uint FrameCounter { get; set; }

        public uint Seed { get; set; }

        public PacketServerSyncMessage(uint frameCounter, uint seed)
        {
            this.FrameCounter = frameCounter;
            this.Seed = seed;
        }
    }
}
