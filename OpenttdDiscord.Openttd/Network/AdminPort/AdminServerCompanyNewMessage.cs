using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminServerCompanyNewMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_NEW;

        public byte CompanyId { get; }

        public AdminServerCompanyNewMessage(byte companyId)
        {
            this.CompanyId = companyId;
        }
    }
}
