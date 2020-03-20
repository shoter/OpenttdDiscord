using OpenttdDiscord.Common;
using RandomAnalyzers.RequiredMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminServerWelcomeMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_WELCOME;

        [RequiredMember]
        public string ServerName { get; set; }

        [RequiredMember]
        public string NetworkRevision { get; set; }

        [RequiredMember]
        public bool IsDedicated { get; set; }

        [RequiredMember]
        public string MapName { get; set; }

        [RequiredMember]
        public uint MapSeed { get; set; }

        [RequiredMember]
        public Landscape Landscape { get; set; }

        [RequiredMember]
        public OttdDate CurrentDate { get; set; }

        [RequiredMember]
        public ushort MapWidth { get; set; }

        [RequiredMember]
        public ushort MapHeight { get; set; }
    }
}
