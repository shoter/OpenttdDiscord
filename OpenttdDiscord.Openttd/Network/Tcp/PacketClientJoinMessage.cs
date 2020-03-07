using RandomAnalyzers.RequiredMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public class PacketClientJoinMessage : ITcpMessage
    {
        public TcpMessageType MessageType => TcpMessageType.PACKET_CLIENT_JOIN;

        [RequiredMember]
        public string OpenttdRevision { get; set; }

        [RequiredMember]
        public uint NewgrfVersion { get; set; }

        [RequiredMember]
        public string ClientName { get; set; }

        public byte JoinAs { get; set; } = 0;

        public byte Language { get; set; } = 0;


    }
}
