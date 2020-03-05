using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Udp
{
    public class PacketUdpServerResponse : IUdpMessage
    {
        public class ActiveNewGrf
        {
            public uint GrfId { get; internal set; }

            public byte[] Md5 { get; internal set; } = new byte[16];
        }

        public UdpMessageType MessageType => UdpMessageType.PACKET_UDP_SERVER_RESPONSE;

        public byte GameVersion { get; internal set; }

        public ActiveNewGrf[] ActiveNewGrfs { get; internal set; }

        public OttdDate GameDate { get; internal set; }
        
        public OttdDate StartDate { get; internal set; }

        public byte CompaniesMax { get; internal set; }
        public byte CompaniesOn { get; internal set; }
        public byte SpectactorsMax { get; internal set; }
        public string ServerName { get; internal set; }
        public string ServerRevision { get; internal set; }
        public byte LanguageId { get; internal set; }
        public OttdLanguage Language => (OttdLanguage)LanguageId;
        public bool HasPassword { get; internal set; }
        public byte ClientsMax { get; internal set; }
        public byte ClientsOn { get; internal set; }
        public byte SpectactorsOn { get; internal set; }

        public string MapName { get; internal set; }
        public ushort MapWidth { get; internal set; }
        public ushort MapHeight { get; internal set; }
        public byte MapSet { get; internal set; }
        public Landscape Landscape => (Landscape)MapSet;
        public bool IsDedicated { get; internal set; }

    }
}
