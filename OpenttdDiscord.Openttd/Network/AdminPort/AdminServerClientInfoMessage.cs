using OpenttdDiscord.Common;
using RandomAnalyzers.RequiredMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminServerClientInfoMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_INFO;

        [RequiredMember]
        public uint ClientId { get; set; }

        [RequiredMember]

        public string Hostname { get; set; }

        [RequiredMember]

        public string ClientName { get; set; }

        [RequiredMember]

        public byte Language { get; set; }

        [RequiredMember]

        public OttdDate JoinDate { get; set; }

        [RequiredMember]

        public byte PlayingAs { get; set; }

    }
}
