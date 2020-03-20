using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminServerDateMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_DATE;

        public OttdDate Date { get; }

        public AdminServerDateMessage(uint date)
        {
            this.Date = new OttdDate(date);
        }
    }
}
