using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminServerCompanyEconomyMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_ECONOMY;

        public byte CompanyId { get; internal set; }

        public ulong Money { get; internal set; }

        public ulong CurrentLoan { get; internal set; }

        public ulong Income { get; internal set; }

        public ushort DeliveredCargo { get; internal set; }

        // there is also data for last 2 quarters - let's skip it for now


    }
}
